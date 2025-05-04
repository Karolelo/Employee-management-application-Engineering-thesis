namespace Repo.Core.Infrastructure;

public interface IDatabaseSeeder
{
    Task getSeedAsync();
}