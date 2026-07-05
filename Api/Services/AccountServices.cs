using Api.Configurations;
using Api.Core.Context;
using Api.Core.Entities;
using Api.Dto;
using Api.Dto.Account;
using Api.Core.Entities.Identity;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

public class AccountServices(
    UserManager<User> userManager,
    SignInManager<User> signInMannger,
    ApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor,
    IUserLoggedService userLoggedService) : IAccountServices
{




    public async Task<(Response<LoginResponseDto?>, short)> LoginAsync(LoginRequestDto request, CancellationToken ct)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return (new Response<LoginResponseDto?>(null, "Credenciais inválidas"), 400);

            var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
                return (new Response<LoginResponseDto?>(null, "Credenciais inválidas"), 400);

            if (user.UserType != UserType.Admin)
                return (new Response<LoginResponseDto?>(null, "Credenciais inválidas"), 400);

            await signInMannger.SignInAsync(user, new AuthenticationProperties
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

            var userRoles = await context.Set<IdentityUserRole<long>>()
                .Where(x => x.UserId == user.Id)
                .ToListAsync(ct);

            var roleId = userRoles
                .Select(x => x.RoleId)
                .FirstOrDefault();

            var claims = await context.Set<RoleClaim>()
                .Where(rc => userRoles.Select(ur => ur.RoleId).Contains(rc.RoleId))
                .Select(rc => rc.ClaimValue!)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToListAsync(ct);

            var response = new LoginResponseDto(
                user.Id,
                user.UserName!,
                user.Email!,
                roleId,
                claims
            );

            return (new Response<LoginResponseDto?>(response, null), 200);
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
                return (new Response<string?>(null, "Contexto HTTP não encontrado"), 500);

            if (!httpContext.Request.Cookies.TryGetValue(ConfigApp.RefreshTokenCookieName, out var currentRefreshToken))
                return (new Response<string?>(null, "Refresh token não encontrado"), 401);

            var storedToken = await context.RefreshToken
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == currentRefreshToken, ct);

            if (storedToken is null || !storedToken.IsActive)
                return (new Response<string?>(null, "Refresh token inválido ou expirado"), 401);

            if (storedToken.User.UserType != UserType.Admin)
                return (new Response<string?>(null, "Refresh token inválido ou expirado"), 401);

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

            await signInMannger.SignInAsync(storedToken.User, new AuthenticationProperties
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
            return (new Response<string?>(null, "Erro ao renovar sessão"), 500);
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

            await signInMannger.SignOutAsync();

            return (new Response<string?>(null, null), 200);
        }
        catch
        {
            return (new Response<string?>(null, "Erro ao realizar logout"), 500);
        }
    }


    public async Task<(Response<LoginResponseDto?>, short)> CheckMe(CancellationToken ct)
    {
        try
        {
            var user = await userLoggedService.GetUserLoggedAsync();

            var userRoles = await context.Set<IdentityUserRole<long>>()
                .Where(x => x.UserId == user.Id)
                .ToListAsync(ct);

            var roleId = userRoles
                .Select(x => x.RoleId)
                .FirstOrDefault();

            var claims = await context.Set<RoleClaim>()
                .Where(rc => userRoles.Select(ur => ur.RoleId).Contains(rc.RoleId))
                .Select(rc => rc.ClaimValue!)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToListAsync(ct);

            var response = new LoginResponseDto(
                user.Id,
                user.UserName!,
                user.Email!,
                roleId,
                claims
            );

            return (new Response<LoginResponseDto?>(response, null), 200);
        }
        catch
        {
            return (new Response<LoginResponseDto?>(null, "Erro ao buscar usuário logado"), 500);
        }
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
