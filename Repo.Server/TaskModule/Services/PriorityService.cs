using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule;

public class PriorityService : IPriorityService
{
    private readonly MyDbContext _context;

    public PriorityService(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<Response<Priority>> AddPriority(PriorityDTO priorityModel)
    {
        try
        {
            if (await _context.Priorities.AnyAsync(p => p.Priority1 == priorityModel.Priority))
                return Response<Priority>.Fail("Priority already exists");

            var priority = new Priority()
            {
                Priority1 = priorityModel.Priority
            };

            _context.Priorities.Add(priority);
            await _context.SaveChangesAsync();

            return Response<Priority>.Ok(priority);
        }
        catch (DbUpdateException e)
        {
            return Response<Priority>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<Priority>.Fail($"Error during creating priority: {e.Message}");
        }
    }

    public Task<Response<PriorityDTO>> UpdatePriority(PriorityDTO priority, int id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Priority>> GetPriorityById(int id)
    {
        throw new NotImplementedException();
    }
}