using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
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

    public async Task<Response<StatusDTO>> UpdateStatus(StatusDTO status, int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<StatusDTO>> GetStatusById(int id)
    {
        throw new NotImplementedException();
    }
}