using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
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
    
    //Methods for getting priority
    public async Task<Response<PriorityDTO>> GetPriorityById(int id)
    {
        var result = await _context.Priorities
            .AsNoTracking()
            .Where(p => p.ID == id)
            .Select(p => new PriorityDTO{ Priority = p.Priority1})
            .FirstOrDefaultAsync();
        return result == null ? Response<PriorityDTO>.Fail("Priority not found") : Response<PriorityDTO>.Ok(result);
    }

    public async Task<Response<IEnumerable<PriorityDTO>>> GetAllPriority()
    {
        var list = await _context.Priorities
            .AsNoTracking()
            .OrderBy(p => p.Priority1)
            .Select(p => new PriorityDTO { Priority = p.Priority1})
            .ToListAsync();

        return list.Count == 0
            ? Response<IEnumerable<PriorityDTO>>.Fail("No priorities found")
            : Response<IEnumerable<PriorityDTO>>.Ok(list);
    }

    public async Task<Response<IEnumerable<PriorityDTO>>> GetPriorityByName(string name)
    {
        var term = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(term))
            return Response<IEnumerable<PriorityDTO>>.Fail("Name cannot be empty");
        
        var list = await _context.Priorities
            .AsNoTracking()
            .Where(p => EF.Functions.Like(p.Priority1, $"%{term}%"))
            .OrderBy(p => p.Priority1)
            .Select(p => new PriorityDTO { Priority = p.Priority1 })
            .ToListAsync();
        
        return list.Count == 0
            ? Response<IEnumerable<PriorityDTO>>.Fail("No priorities match the query")
            : Response<IEnumerable<PriorityDTO>>.Ok(list);
    }
    
    //Methods for creating priority
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

    //Methods for updating priority
    public async Task<Response<PriorityDTO>> UpdatePriority(PriorityDTO priorityModel, int id)
    {
        try
        {
            var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.ID == id);
            if (priority == null)
                return Response<PriorityDTO>.Fail("Priority not found");
            
            var newPriority = (priorityModel.Priority ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(newPriority))
                return Response<PriorityDTO>.Fail("Priority name cannot be empty");
            
            var exists = await _context.Priorities
                .AnyAsync(p => p.ID != id && p.Priority1 == newPriority);
            if (exists)
                return Response<PriorityDTO>.Fail("Priority with this name already exists");
            
            priority.Priority1 = newPriority;
            await _context.SaveChangesAsync();

            var dto = new PriorityDTO
            {
                Priority = priority.Priority1
            };
            
            return Response<PriorityDTO>.Ok(dto);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<PriorityDTO>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<PriorityDTO>.Fail($"Error during updating priority: {e.Message}");
        }
    }
}