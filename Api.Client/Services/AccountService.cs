using Api.Client.Configurations;
using Api.Client.Dto;
using Api.Client.Dto.Account;
using Api.Client.Services.Abstractions;
using Api.Core.Context;
using Api.Core.Entities;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Api.Client.Services;

public class AccountService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<(Response<LoginResponseDto?>, short)> LoginAsync(LoginRequestDto request, CancellationToken ct)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return (new Response<LoginResponseDto?>(null, "Credenciais invalidas"), 400);

            var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
                return (new Response<LoginResponseDto?>(null, "Credenciais invalidas"), 400);

            if (user.UserType != UserType.Client)
                return (new Response<LoginResponseDto?>(null, "Credenciais invalidas"), 400);

            var clientUser = await context.ClientUser
                .AsNoTracking()
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

            if (clientUser is null)
                return (new Response<LoginResponseDto?>(null, "Cliente sem empresa vinculada"), 403);

            await signInManager.SignInAsync(user, new AuthenticationProperties
            {
                IsPersistent = request.RememberMe,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(ConfigApp.TokenCookieTime)
            });

            var newRefreshToken = GenerateRefreshToken();

            var storedRefreshToken = await context.RefreshToken
                .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

            if (storedRefreshToken is null)
            {
                storedRefreshToken = new RefreshToken(newRefreshToken, user.Id);
                await context.RefreshToken.AddAsync(storedRefreshToken, ct);
            }
            else
            {
                storedRefreshToken.Replace(newRefreshToken);
            }

            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

            var storedSession = await context.UserSession
                .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

            if (storedSession is null)
            {
                storedSession = new UserSession(user.Id, ipAddress, userAgent);
                await context.UserSession.AddAsync(storedSession, ct);
            }
            else
            {
                storedSession.Refresh(ipAddress, userAgent);
            }

            await context.SaveChangesAsync(ct);

            AppendRefreshTokenCookie(newRefreshToken);

            return (new Response<LoginResponseDto?>(BuildResponse(user, clientUser), null), 200);
        }
        catch
        {
            return (new Response<LoginResponseDto?>(null, "Erro interno ao realizar login"), 500);
        }
    }

    public async Task<(Response<string?>, short)> RefreshTokenAsync(CancellationToken ct)
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext is null)
                return (new Response<string?>(null, "Contexto HTTP nao encontrado"), 500);

            if (!httpContext.Request.Cookies.TryGetValue(ConfigApp.RefreshTokenCookieName, out var currentRefreshToken))
                return (new Response<string?>(null, "Refresh token nao encontrado"), 401);

            var storedToken = await context.RefreshToken
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == currentRefreshToken, ct);

            if (storedToken is null || !storedToken.IsActive)
                return (new Response<string?>(null, "Refresh token invalido ou expirado"), 401);

            if (storedToken.User.UserType != UserType.Client)
                return (new Response<string?>(null, "Refresh token invalido ou expirado"), 401);

            var clientProfileExists = await context.ClientUser
                .AsNoTracking()
                .AnyAsync(x => x.UserId == storedToken.UserId, ct);

            if (!clientProfileExists)
                return (new Response<string?>(null, "Cliente sem empresa vinculada"), 403);

            var newRefreshToken = GenerateRefreshToken();
            storedToken.Replace(newRefreshToken);

            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            var storedSession = await context.UserSession
                .FirstOrDefaultAsync(x => x.UserId == storedToken.UserId, ct);

            if (storedSession is null)
            {
                storedSession = new UserSession(storedToken.UserId, ipAddress, userAgent);
                await context.UserSession.AddAsync(storedSession, ct);
            }
            else
            {
                storedSession.Refresh(ipAddress, userAgent);
            }

            await signInManager.SignInAsync(storedToken.User, new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(ConfigApp.TokenCookieTime)
            });

            await context.SaveChangesAsync(ct);

            AppendRefreshTokenCookie(newRefreshToken);

            return (new Response<string?>(null, null), 200);
        }
        catch
        {
            return (new Response<string?>(null, "Erro ao renovar sessao"), 500);
        }
    }

    public async Task<(Response<string?>, short)> LogoutAsync(CancellationToken ct)
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext != null &&
                httpContext.Request.Cookies.TryGetValue(ConfigApp.RefreshTokenCookieName, out var refreshToken))
            {
                var storedToken = await context.RefreshToken
                    .FirstOrDefaultAsync(x => x.Token == refreshToken, ct);

                if (storedToken != null && storedToken.IsActive)
                {
                    storedToken.Revoke();
                }

                if (storedToken != null)
                {
                    var storedSession = await context.UserSession
                        .FirstOrDefaultAsync(x => x.UserId == storedToken.UserId, ct);

                    if (storedSession != null && storedSession.IsActive)
                    {
                        storedSession.Revoke();
                    }
                }

                await context.SaveChangesAsync(ct);
                httpContext.Response.Cookies.Delete(ConfigApp.RefreshTokenCookieName);
            }

            await signInManager.SignOutAsync();

            return (new Response<string?>(null, null), 200);
        }
        catch
        {
            return (new Response<string?>(null, "Erro ao realizar logout"), 500);
        }
    }

    public async Task<(Response<LoginResponseDto?>, short)> CheckMeAsync(CancellationToken ct)
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext is null)
                return (new Response<LoginResponseDto?>(null, "Contexto HTTP nao encontrado"), 500);

            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null || user.UserType != UserType.Client)
                return (new Response<LoginResponseDto?>(null, "Usuario nao autenticado"), 401);

            var clientUser = await context.ClientUser
                .AsNoTracking()
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.UserId == user.Id, ct);

            if (clientUser is null)
                return (new Response<LoginResponseDto?>(null, "Cliente sem empresa vinculada"), 403);

            return (new Response<LoginResponseDto?>(BuildResponse(user, clientUser), null), 200);
        }
        catch
        {
            return (new Response<LoginResponseDto?>(null, "Erro ao buscar usuario logado"), 500);
        }
    }

    private static LoginResponseDto BuildResponse(User user, ClientUser clientUser)
    {
        var tenantId = clientUser.CompanyId.ToString();

        return new LoginResponseDto(
            user.Id,
            user.UserName!,
            user.Email!,
            user.UserType.ToString(),
            clientUser.CompanyId,
            clientUser.Company.Name,
            tenantId
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        httpContext.Response.Cookies.Append(ConfigApp.RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(ConfigApp.RefreshTokenCookieTime)
        });
    }
}
