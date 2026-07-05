using Api.Configurations.Identity;
using Api.Core.Context;
using Api.Dto;
using Api.Dto.RoleClaim;
using Api.Core.Entities.Identity;
using Api.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class RoleClaimService(ApplicationDbContext context) : IRoleClaimService
{
    public async Task<(Response<RoleClaimResponseDto?>, short)> GetByRoleIdAsync(long roleId, CancellationToken ct)
    {
        try
        {
            var role = await context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId, ct);

            if (role is null)
            {
                return (new Response<RoleClaimResponseDto?>(null, "Role não encontrada."), 404);
            }

            var existingClaims = await context.Set<RoleClaim>()
                .AsNoTracking()
                .Where(rc => rc.RoleId == roleId && rc.ClaimType == Permissions.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(ct);

            var availableClaims = Permissions.GetPermissions()
                .Where(p => p.ClaimType == Permissions.Permission)
                .Select(p => new RoleClaimItemDto(
                    p.PermissionName,
                    p.ClaimType,
                    p.Description,
                    existingClaims.Contains(p.PermissionName)
                ))
                .ToList();

            var response = new RoleClaimResponseDto(
                role.Id,
                role.Name ?? string.Empty,
                availableClaims
            );

            return (new Response<RoleClaimResponseDto?>(response, null), 200);
        }
        catch
        {
            return (new Response<RoleClaimResponseDto?>(null, "Erro ao recuperar claims da role."), 500);
        }
    }

    public async Task<(Response<bool>, short)> UpdateAsync(long roleId, UpdateRoleClaimRequestDto request, CancellationToken ct)
    {
        try
        {
            var roleExists = await context.Roles
                .AsNoTracking()
                .AnyAsync(r => r.Id == roleId, ct);

            if (!roleExists)
            {
                return (new Response<bool>(false, "Perfil não encontrada."), 404);
            }

            var validPermissions = Permissions.GetPermissions()
                .Where(p => p.ClaimType == Permissions.Permission)
                .Select(p => p.PermissionName)
                .ToHashSet();

            var requestedClaims = request.Claims
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList();

            var invalidClaims = requestedClaims
                .Where(c => !validPermissions.Contains(c))
                .ToList();

            if (invalidClaims.Count > 0)
            {
                return (new Response<bool>(false, "Uma ou mais permissões informadas são inválidas."), 400);
            }

            var existingClaims = await context.Set<RoleClaim>()
                .Where(rc => rc.RoleId == roleId && rc.ClaimType == Permissions.Permission)
                .ToListAsync(ct);

            context.RemoveRange(existingClaims);

            var descriptions = Permissions.GetPermissions()
                .Where(p => p.ClaimType == Permissions.Permission)
                .ToDictionary(p => p.PermissionName, p => p.Description);

            var newClaims = requestedClaims
                .Select(claim => new RoleClaim
                {
                    RoleId = roleId,
                    ClaimType = Permissions.Permission,
                    ClaimValue = claim
                })
                .ToList();

            await context.AddRangeAsync(newClaims, ct);
            await context.SaveChangesAsync(ct);

            return (new Response<bool>(true, "Permissões atualizadas com sucesso."), 200);
        }
        catch
        {
            return (new Response<bool>(false, "Erro ao atualizar permissões"), 500);
        }
    }
}
