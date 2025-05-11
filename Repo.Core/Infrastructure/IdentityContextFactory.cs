using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Repo.Core.Infrastructure;

public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    public IdentityContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<IdentityContext>();
        optionBuilder.UseSqlServer("Server=localhost,1433;Database=Tmp;User=sa;Password=Haslo1234*;TrustServerCertificate=True;");
        return new IdentityContext(optionBuilder.Options);
    }
}