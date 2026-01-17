using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule.Repository;

public class StatusRepository(MyDbContext context) : IStatusRepository
{
    
    public async Task<Status?> GetStatusByName(string name)
    {
        return await context.Statuses.FirstOrDefaultAsync(s => s.Status1 == name);
    }
}