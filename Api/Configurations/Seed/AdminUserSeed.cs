using Api.Configurations.Identity;
using Api.Configurations.Seed.Abstraction;
using Api.Core.Context;
using Api.Core.Entities;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Configurations.Seed;

public sealed class AdminUserSeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        const string email = "admin@teste.com";
        const string password = "Lp59bh5Qa24hfI6SsTepaoBrs0ZBKqyz*";

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new User("Admin", email, UserType.Admin);

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao criar usuario admin: {errors}");
            }
        }
        else if (user.UserType != UserType.Admin)
        {
            user.SetUserType(UserType.Admin);
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao atualizar usuario admin: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, Permissions.Admin))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(user, Permissions.Admin);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addToRoleResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao vincular admin a role '{Permissions.Admin}': {errors}");
            }
        }

        var profileExists = await context.AdminUser
            .AnyAsync(x => x.UserId == user.Id, ct);

        if (profileExists)
            return;

        await context.AdminUser.AddAsync(new AdminUser(user.Id, SeedDefaults.CompanyId), ct);
        await context.SaveChangesAsync(ct);
    }
}

