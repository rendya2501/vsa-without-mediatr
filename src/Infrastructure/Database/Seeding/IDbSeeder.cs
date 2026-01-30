namespace Infrastructure.Database.Seeding;

public interface IDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
