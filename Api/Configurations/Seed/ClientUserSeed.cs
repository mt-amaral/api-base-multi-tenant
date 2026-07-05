using Api.Configurations.Seed.Abstraction;
using Api.Core.Context;
using Api.Core.Entities;
using Api.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Configurations.Seed;

public sealed class ClientUserSeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        const string email = "client@teste.com";
        const string password = "Lp59bh5Qa24hfI6SsTepaoBrs0ZBKqyz*";

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new User("Client", email, UserType.Client);

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao criar usuario client: {errors}");
            }
        }
        else if (user.UserType != UserType.Client)
        {
            user.SetUserType(UserType.Client);
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                throw new Exception($"Erro ao atualizar usuario client: {errors}");
            }
        }

        var profileExists = await context.ClientUser
            .AnyAsync(x => x.UserId == user.Id, ct);

        if (profileExists)
            return;

        await context.ClientUser.AddAsync(new ClientUser(user.Id, SeedDefaults.CompanyId), ct);
        await context.SaveChangesAsync(ct);
    }
}
