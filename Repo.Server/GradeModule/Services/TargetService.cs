using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Services;

public class TargetService : ITargetService
{
    private readonly MyDbContext _context;

    public TargetService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Response<ICollection<TargetDTO>>> GetTargets(string? q, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<TargetDTO>> GetTargetById(int id)
    {
        var target = await _context.Targets.AsNoTracking()
            .Where(t => t.ID == id)
            .Select(t => new TargetDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Finish_Time = t.Finish_Time,
                Tag = t.Tag.Name
            })
            .FirstOrDefaultAsync();
        
        return target == null
            ? Response<TargetDTO>.Fail("Target not found")
            : Response<TargetDTO>.Ok(target);
    }

    public async Task<Response<ICollection<TargetDTO>>> GetUserTargets(int userId)
    {
        var targets = await _context.Targets.AsNoTracking()
            .Where(t => t.Users.Any(u => u.ID == userId))
            .Select(t => new TargetDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Finish_Time = t.Finish_Time,
                Tag = t.Tag.Name
            })
            .ToListAsync();
        
        return targets.Count == 0
            ? Response<ICollection<TargetDTO>>.Fail("User has no targets")
            : Response<ICollection<TargetDTO>>.Ok(targets);
    }

    public async Task<Response<TargetDTO>> CreateTarget(int userId, TargetMiniDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<TargetDTO>> UpdateTarget(int userId, TargetDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<TargetDTO>> DeleteTarget(int targetId)
    {
        throw new NotImplementedException();
    }
}