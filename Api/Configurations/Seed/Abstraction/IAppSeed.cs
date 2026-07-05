namespace Api.Configurations.Seed.Abstraction;

public interface IAppSeed
{
    Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct = default);
}