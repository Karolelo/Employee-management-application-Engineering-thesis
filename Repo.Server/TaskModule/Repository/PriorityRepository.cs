using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule.Repository;

public class PriorityRepository(MyDbContext context) : IPriorityRepository
{
    
    public async Task<Priority?> GetPriorityByName(string name)
    {
        return await context.Priorities.FirstOrDefaultAsync(p => p.Priority1 == name);
    }
}