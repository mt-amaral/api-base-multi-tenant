using Api.Configurations.Identity;
using Api.Configurations.Seed.Abstraction;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public sealed class RoleClaimSeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        await AddAllPermissionsToRoleAsync(
            roleManager,
            Permissions.Admin);
    }

    private static async Task AddAllPermissionsToRoleAsync(
        RoleManager<Role> roleManager,
        string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
            throw new Exception($"Role '{roleName}' não encontrada para adicionar permission claims.");

        var existingClaims = await roleManager.GetClaimsAsync(role);

        foreach (var permission in Permissions.GetPermissions())
        {
            var alreadyExists = existingClaims.Any(c =>
                c.Type == permission.ClaimType &&
                c.Value == permission.PermissionName);

            if (alreadyExists)
                continue;

            var result = await roleManager.AddClaimAsync(role, new Claim(permission.ClaimType, permission.PermissionName));

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(
                    $"Erro ao adicionar claim '{permission.ClaimType}={permission.PermissionName}' para role '{roleName}': {errors}");
            }
        }
    }
}