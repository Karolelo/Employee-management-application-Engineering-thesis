namespace Repo.Core.Infrastructure;

public interface IDatabaseSeeder
{
    Task GetSeedAsync();
}