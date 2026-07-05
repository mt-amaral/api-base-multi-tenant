using Api.Core.Context;
using Api.Dto;
using Api.Dto.User;
using Api.Core.Entities.Identity;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class UserService(ApplicationDbContext context,
    UserManager<User> userManager, IUserLoggedService userLoggedService) : IUserService
{

    public async Task<(PagedResponse<List<UserResponseDto>?>, short)> GetUsersAsync(FilterUsersRequestDto request, CancellationToken ct)
    {
        try
        {
            var query =
                from u in context.User.AsNoTracking()
                join ur in context.Set<IdentityUserRole<long>>().AsNoTracking()
                    on u.Id equals ur.UserId
                orderby u.Id
                select new
                {
                    u.Id,
                    Name = u.UserName!,
                    Email = u.Email!,
                    ur.RoleId
                };

            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    x.Email.ToLower().Contains(search));
            }

            if (request.RoleId > 0)
            {
                query = query.Where(x => x.RoleId == request.RoleId);
            }

            var totalCount = await query.CountAsync(ct);

            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserResponseDto(
                    x.Id,
                    x.Name,
                    x.Email,
                    x.RoleId
                ))
                .ToListAsync(ct);

            return (
                new PagedResponse<List<UserResponseDto>?>(users, totalCount, request.PageNumber, request.PageSize, null),
                200
            );
        }
        catch (Exception)
        {
            return (
                new PagedResponse<List<UserResponseDto>?>(null, 0, request.PageNumber, request.PageSize, "Erro ao recuperar usuários"),
                500
            );
        }
    }

    public async Task<(Response<UserResponseDto?>, short)> CreateAsync(CreateRequestDto request, CancellationToken ct)
    {
        try
        {
            var entity = await userManager.FindByEmailAsync(request.Email);
            if (entity != null)
                return (new Response<UserResponseDto?>(null, "Já existe um usuário registrado com esse email."), 400);

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return (new Response<UserResponseDto?>(null, "Senha é obrigatória."), 400);

            if (request.NewPassword != request.ConfirmPassword)
                return (new Response<UserResponseDto?>(null, "As senhas não conferem."), 400);

            var roleExists = await RoleExistsAsync(request.RoleId, ct);
            if (!roleExists)
                return (new Response<UserResponseDto?>(null, "Perfil não encontrado."), 400);

            var user = new User(userName: request.UserName, email: request.Email);

            var statusCreate = await userManager.CreateAsync(user, request.NewPassword);
            if (!statusCreate.Succeeded)
                return (new Response<UserResponseDto?>(null, $"Erro ao criar usuário {user.UserName}"), 400);

            context.Add(new IdentityUserRole<long>
            {
                UserId = user.Id,
                RoleId = request.RoleId
            });

            await context.SaveChangesAsync(ct);

            var response = new UserResponseDto(
                user.Id,
                user.UserName!,
                user.Email!,
                request.RoleId
            );

            return (new Response<UserResponseDto?>(response, $"Usuário {user.UserName} registrado com sucesso!"), 200);
        }
        catch
        {
            return (new Response<UserResponseDto?>(null, $"Erro na criação de usuário: {request.Email}"), 500);
        }
    }

    public async Task<(Response<UserResponseDto?>, short)> UpdateAsnc(long userId, UpdateUserRequestDto userRequest, CancellationToken ct)
    {
        try
        {
            var entity = await userManager.Users
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (entity == null)
                return (new Response<UserResponseDto?>(null, "Usuário não encontrado"), 404);

            return await UpdateUserAsync(entity, userRequest, ct);
        }
        catch
        {
            return (new Response<UserResponseDto?>(null, $"Erro na atualização de usuário: {userRequest.Email}"), 500);
        }
    }

    public async Task<(Response<UserResponseDto?>, short)> UpdateLoggedAsnc(UpdateUserRequestDto request, CancellationToken ct)
    {
        try
        {
            var entity = await userLoggedService.GetUserLoggedAsync();

            return await UpdateUserAsync(entity, request, ct);
        }
        catch
        {
            return (new Response<UserResponseDto?>(null, $"Erro na atualização de usuário: {request.Email}"), 500);
        }
    }

    private async Task<(Response<UserResponseDto?>, short)> UpdateUserAsync(
        User entity,
        UpdateUserRequestDto userRequest,
        CancellationToken ct)
    {
        entity.UserName = userRequest.UserName;
        entity.Email = userRequest.Email;

        var updateUserResult = await userManager.UpdateAsync(entity);
        if (!updateUserResult.Succeeded)
        {
            var errorMessage = string.Join(" | ", updateUserResult.Errors.Select(x => x.Description));
            return (new Response<UserResponseDto?>(null, errorMessage), 400);
        }

        var passwordResult = await UpdatePasswordIfNeededAsync(entity, userRequest);
        if (passwordResult is not null)
            return passwordResult.Value;

        var roleResult = await UpdateRoleIfNeededAsync(entity.Id, userRequest.RoleId, ct);
        if (roleResult is not null)
            return roleResult.Value;

        var roleId = await context.Set<IdentityUserRole<long>>()
            .AsNoTracking()
            .Where(x => x.UserId == entity.Id)
            .Select(x => x.RoleId)
            .FirstOrDefaultAsync(ct);

        var response = new UserResponseDto(
            entity.Id,
            entity.UserName!,
            entity.Email!,
            roleId
        );

        return (new Response<UserResponseDto?>(response, $"Usuário {response.Name} atualizado com sucesso"), 200);
    }


    public async Task<(Response<bool>, short)> DeleteAsync(long userId, CancellationToken ct)
    {
        try
        {
            var entity = await context.User
                .FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (entity == null)
                return (new Response<bool>(false, "Usuário não encontrado"), 404);

            var result = await userManager.DeleteAsync(entity);

            if (!result.Succeeded)
                return (new Response<bool>(false, "Erro ao remover usuário"), 400);

            return (new Response<bool>(true, "Usuário removido com sucesso"), 200);
        }
        catch
        {
            return (new Response<bool>(false, "Erro ao remover usuário"), 500);
        }
    }


    private async Task<(Response<UserResponseDto?>, short)?> UpdateRoleIfNeededAsync(
        long userId,
        long? roleId,
        CancellationToken ct)
    {
        if (!roleId.HasValue || roleId.Value <= 0)
            return null;

        var roleExists = await RoleExistsAsync(roleId.Value, ct);
        if (!roleExists)
            return (new Response<UserResponseDto?>(null, "Perfil não encontrado"), 400);

        var userRoles = await context.Set<IdentityUserRole<long>>()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

        if (userRoles.Count > 0)
            context.RemoveRange(userRoles);

        context.Add(new IdentityUserRole<long>
        {
            UserId = userId,
            RoleId = roleId.Value
        });

        return null;
    }

    private async Task<(Response<UserResponseDto?>, short)?> UpdatePasswordIfNeededAsync(
        User entity,
        UpdateUserRequestDto userRequest)
    {
        if (string.IsNullOrWhiteSpace(userRequest.NewPassword))
            return null;

        var token = await userManager.GeneratePasswordResetTokenAsync(entity);
        var passwordResult = await userManager.ResetPasswordAsync(entity, token, userRequest.NewPassword);

        if (passwordResult.Succeeded)
            return null;

        return (new Response<UserResponseDto?>(null, "Erro ao atualizar senha"), 400);
    }


    private async Task<bool> RoleExistsAsync(long roleId, CancellationToken ct)
    {
        return await context.Roles
            .AsNoTracking()
            .AnyAsync(x => x.Id == roleId, ct);
    }
}

