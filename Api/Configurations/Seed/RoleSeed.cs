using Api.Configurations.Identity;
using Api.Configurations.Seed.Abstraction;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Api.Configurations.Seed;

public sealed class RoleSeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

        var roles = new[]
        {
            new Role { Name = Permissions.Admin,   NormalizedName = Permissions.Admin.ToUpper(),   Description = "Acesso total" },
        };

        foreach (var roleSeed in roles)
        {
            var existingRole = await roleManager.FindByNameAsync(roleSeed.Name!);

            if (existingRole is null)
            {
                var createResult = await roleManager.CreateAsync(roleSeed);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar role '{roleSeed.Name}': {errors}");
                }

                continue;
            }

            var mustUpdate =
                existingRole.Description != roleSeed.Description ||
                existingRole.NormalizedName != roleSeed.NormalizedName;

            if (!mustUpdate)
                continue;

            existingRole.Description = roleSeed.Description;
            existingRole.NormalizedName = roleSeed.NormalizedName;

            var updateResult = await roleManager.UpdateAsync(existingRole);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao atualizar role '{existingRole.Name}': {errors}");
            }
        }
    }
}