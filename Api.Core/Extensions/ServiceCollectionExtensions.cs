using Api.Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDatabase(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string do banco de dados não configurada.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

        return services;
    }
}
