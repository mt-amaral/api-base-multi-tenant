using Api.Configurations.Seed.Abstraction;
using Api.Core.Context;
using Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Configurations.Seed;

public sealed class CompanySeed : IAppSeed
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var company = await context.Company
            .FirstOrDefaultAsync(x => x.Id == SeedDefaults.CompanyId, ct);

        if (company is null)
        {
            await context.Company.AddAsync(new Company(SeedDefaults.CompanyId, SeedDefaults.CompanyName), ct);
            await context.SaveChangesAsync(ct);
            return;
        }

        if (company.Name == SeedDefaults.CompanyName)
            return;

        company.Rename(SeedDefaults.CompanyName);
        await context.SaveChangesAsync(ct);
    }
}
