using Api.Core.Context;
using Api.Core.Entities.Identity;
using Api.Dto;
using Api.Dto.Role;
using Api.Services.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class RoleServices(ApplicationDbContext context) : IRoleService
{
    public async Task<(PagedResponse<List<GetAllRoleResponseDto>?>, short)> ListRolesPageAsync(FilterRoleRequestDto request, CancellationToken ct)
    {
        try
        {
            var query = context.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                var search = request.SearchString.Trim().ToLower();

                query = query.Where(r =>
                    (r.Name != null && r.Name.ToLower().Contains(search)) ||
                    (r.Description != null && r.Description.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(ct);

            var roles = await query
                .OrderBy(r => r.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new GetAllRoleResponseDto(
                    r.Id,
                    r.Name ?? string.Empty,
                    r.Description ?? string.Empty,
                    context.Set<IdentityUserRole<long>>().Count(ur => ur.RoleId == r.Id)
                ))
                .ToListAsync(ct);

            return (
                new PagedResponse<List<GetAllRoleResponseDto>?>(roles, totalCount, request.PageNumber, request.PageSize, null),
                200
            );
        }
        catch
        {
            return (
                new PagedResponse<List<GetAllRoleResponseDto>?>(null, 0, request.PageNumber, request.PageSize, "Erro ao listar Perfis!"),
                500
            );
        }
    }

    public async Task<(Response<List<RoleResponseDto>?>, short)> ListAllRolesAsync(CancellationToken ct)
    {
        try
        {
            var result = new List<RoleResponseDto>();
            var roles = await context.Roles.AsNoTracking().ToListAsync(ct);
            foreach (var role in roles)
            {
                result.Add(new RoleResponseDto(role.Id, role.Name!, role.Description!));
            }
            return (new Response<List<RoleResponseDto>?>(result, null), 200);
        }
        catch
        {
            return (new Response<List<RoleResponseDto>?>(null, $"Erro ao listar Perfis!"), 500);
        }
    }



    public async Task<(Response<RoleResponseDto?>, short)> CreateAsync(CreateRoleRequestDto request, CancellationToken ct)
    {
        try
        {
            var normalizedName = request.Name.Trim().ToUpper();

            var roleExists = await context.Roles
                .AsNoTracking()
                .AnyAsync(r => r.NormalizedName == normalizedName, ct);

            if (roleExists)
            {
                return (new Response<RoleResponseDto?>(null, "Já existe uma Perfil com esse nome."), 400);
            }

            var role = new Role
            {
                Name = request.Name.Trim(),
                NormalizedName = normalizedName,
                Description = request.Description?.Trim()
            };

            await context.Roles.AddAsync(role, ct);
            await context.SaveChangesAsync(ct);

            var response = new RoleResponseDto(
                role.Id,
                role.Name ?? string.Empty,
                role.Description ?? string.Empty
            );

            return (new Response<RoleResponseDto?>(response, "Perfil criado com sucesso."), 201);
        }
        catch
        {
            return (new Response<RoleResponseDto?>(null, "Erro ao criar Perfil!"), 500);
        }
    }

    public async Task<(Response<RoleResponseDto?>, short)> UpdateAsync(long id, UpdateRoleRequestDto request, CancellationToken ct)
    {
        try
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (role is null)
            {
                return (new Response<RoleResponseDto?>(null, "Perfil não encontrada."), 404);
            }

            var normalizedName = request.Name.Trim().ToUpper();

            var duplicateRole = await context.Roles
                .AsNoTracking()
                .AnyAsync(r => r.Id != id && r.NormalizedName == normalizedName, ct);

            if (duplicateRole)
            {
                return (new Response<RoleResponseDto?>(null, "Já existe um Perfil com esse nome."), 400);
            }

            role.Name = request.Name.Trim();
            role.NormalizedName = normalizedName;
            role.Description = request.Description?.Trim();

            context.Roles.Update(role);
            await context.SaveChangesAsync(ct);

            var response = new RoleResponseDto(
                role.Id,
                role.Name ?? string.Empty,
                role.Description ?? string.Empty
            );

            return (new Response<RoleResponseDto?>(response, "Perfil atualizado com sucesso."), 200);
        }
        catch
        {
            return (new Response<RoleResponseDto?>(null, "Erro ao atualizar Perfil!"), 500);
        }
    }

    public async Task<(Response<bool>, short)> DeleteAsync(long id, CancellationToken ct)
    {
        try
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            if (role is null)
            {
                return (new Response<bool>(false, "Perfil não encontrada."), 404);
            }

            var roleInUse = await context.Set<IdentityUserRole<long>>()
                .AsNoTracking()
                .AnyAsync(ur => ur.RoleId == id, ct);

            if (roleInUse)
            {
                return (new Response<bool>(false, "Não é possível remover o Perfil vinculado a usuários."), 400);
            }

            context.Roles.Remove(role);
            await context.SaveChangesAsync(ct);

            return (new Response<bool>(true, "Perfil removido com sucesso."), 200);
        }
        catch
        {
            return (new Response<bool>(false, "Erro ao remover Perfil!"), 500);
        }
    }
}
