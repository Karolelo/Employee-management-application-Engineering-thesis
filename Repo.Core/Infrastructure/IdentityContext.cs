using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Models;
using Task = Repo.Core.Models.Task;
using Type = Repo.Core.Models.Type;
namespace Repo.Core.Infrastructure;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //Zastanawiam się, bo powiniśmy użyć basic entity, spoko jest to praktyka, 
        // Czy jak to zrobimy mogę wstawić tu basic entity i nie masz tych ignore jebanhych 
        builder.Ignore<Task>();
        builder.Ignore<RelatedTask>();
        builder.Ignore<Priority>();
        builder.Ignore<Status>();
        builder.Ignore<WorkTask>();
        builder.Ignore<Group>();
        builder.Ignore<AbsenceDay>();
        builder.Ignore<Application>();
        builder.Ignore<BasicEntity>();
        builder.Ignore<Benefit>();
        builder.Ignore<BenefitType>();
        builder.Ignore<Contribution>();
        builder.Ignore<Course>();
        builder.Ignore<Document>();
        builder.Ignore<DocumentType>();
        builder.Ignore<HireHelper>();
        builder.Ignore<Skill>();
        builder.Ignore<Target>();
        builder.Ignore<Type>();
        builder.Ignore<Grade>();
        builder.Ignore<WorkTable>();
    }
    
    
}