using Api.Configurations.Seed.Abstraction;
using Api.Core.Context;
using Microsoft.EntityFrameworkCore;

namespace Api.Configurations.Setup;

public static class SeedSetup
{
    public static async Task UseSeedSetupAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            var seeds = services.GetServices<IAppSeed>();
            foreach (var seed in seeds)
            {
                await seed.SeedAsync(services);
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Erro ao aplicar migrações ou seeds no ambiente Staging.");
        }
    }
}
