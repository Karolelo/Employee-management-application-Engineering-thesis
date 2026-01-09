using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule;

public class StatusService : IStatusService
{
    private readonly MyDbContext _context;

    public StatusService(MyDbContext context)
    {
        _context = context;
    }
    
    //Methods for getting status
    public async Task<Response<StatusDTO>> GetStatusById(int id)
    {
        var result = await _context.Statuses
            .AsNoTracking()
            .Where(s => s.ID == id)
            .Select(s => new StatusDTO { Status = s.Status1})
            .FirstOrDefaultAsync();
        return result == null ? Response<StatusDTO>.Fail("Status not found") : Response<StatusDTO>.Ok(result);
    }

    public async Task<Response<ICollection<StatusDTO>>> GetAllStatus()
    {
        var list = await _context.Statuses
            .AsNoTracking()
            .OrderBy(s => s.Status1)
            .Select(s => new StatusDTO { Status = s.Status1 })
            .ToListAsync();
        
        return list.Count == 0
            ? Response<ICollection<StatusDTO>>.Fail("No statuses found")
            : Response<ICollection<StatusDTO>>.Ok(list);
    }

    public async Task<Response<ICollection<StatusDTO>>> GetStatusByName(string? name)
    {
        var term = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(term))
        {
            return Response<ICollection<StatusDTO>>.Fail("Name cannot be empty");
        }
        
        var list = await _context.Statuses
            .AsNoTracking()
            .Where(s => EF.Functions.Like(s.Status1, $"%{term}%"))
            .OrderBy(s => s.Status1)
            .Select(s => new StatusDTO { Status = s.Status1 })
            .ToListAsync();
        
        return list.Count == 0
            ? Response<ICollection<StatusDTO>>.Fail("No statuses match the query")
            : Response<ICollection<StatusDTO>>.Ok(list);
    }
    
    //Methods for creating status
    public async Task<Response<Status>> AddStatus(StatusDTO statusModel)
    {
        try
        {
            if (await _context.Statuses.AnyAsync(s => s.Status1 == statusModel.Status))
                return Response<Status>.Fail("Status with same name already exists");

            var status = new Status()
            {
                Status1 = statusModel.Status
            };

            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            return Response<Status>.Ok(status);
        }
        catch (DbUpdateException e)
        {
            return Response<Status>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<Status>.Fail($"Error during creating status: {e.Message}");
        }
    }

    //Methods for updating status
    public async Task<Response<StatusDTO>> UpdateStatus(StatusDTO statusModel, int id)
    {
        try
        {
            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.ID == id);
            if (status == null)
                return Response<StatusDTO>.Fail("Status not found");
            
            var newStatus = (statusModel.Status ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(newStatus))
                return Response<StatusDTO>.Fail("Status name cannot be empty");
            
            var exists = await _context.Statuses
                .AnyAsync(s => s.ID != id && s.Status1 == newStatus);
            if (exists)
                return Response<StatusDTO>.Fail("Priority with this name already exists");
            
            status.Status1 = newStatus;
            await _context.SaveChangesAsync();

            var dto = new StatusDTO
            {
                Status = status.Status1
            };
            
            return Response<StatusDTO>.Ok(dto);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<StatusDTO>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<StatusDTO>.Fail($"Error during updating status: {e.Message}");
        }
    }
}