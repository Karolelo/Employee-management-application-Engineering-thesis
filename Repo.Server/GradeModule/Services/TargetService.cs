using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
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
    
    //[HttpGet] methods
    public async Task<Response<ICollection<TargetDTO>>> GetTargets(string? q, int page, int pageSize)
    {
        var query = _context.Targets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(t => t.Name.Contains(q));
        }
        
        var data = await query
            .OrderByDescending(t => t.Start_Time)
            .ThenByDescending(t => t.Finish_Time)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TargetDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Finish_Time = t.Finish_Time,
                //Tag = t.Tag.Name
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<TargetDTO>>.Fail("No targets found")
            : Response<ICollection<TargetDTO>>.Ok(data);
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

    //[HttpPost] methods
    public async Task<Response<TargetDTO>> CreateTarget(int userId, TargetMiniDTO dto)
    {
        try
        {
            var user = await _context.Users.Include(u => u.Targets).FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                return Response<TargetDTO>.Fail("User not found");
            
            var name = (dto.Name ?? string.Empty).Trim();
            var desc = (dto.Description ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Response<TargetDTO>.Fail("Name is required");
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                return Response<TargetDTO>.Fail("Description is required");
            }

            if (dto.Finish_Time < dto.Start_Time)
            {
                return Response<TargetDTO>.Fail("Finish time cannot be earlier than Start time");
            }

            var exists = await _context.Targets.AnyAsync(t => t.Name == name);
            if (exists)
            {
                return Response<TargetDTO>.Fail("Target with this name already exists");
            }

            var target = new Target
            {
                Name = name,
                Description = desc,
                Start_Time = dto.Start_Time,
                Finish_Time = dto.Finish_Time
            };

            target.Users.Add(user);
            _context.Targets.Add(target);
            await _context.SaveChangesAsync();

            var result = new TargetDTO
            {
                ID = target.ID,
                Name = target.Name,
                Description = target.Description,
                Start_Time = target.Start_Time,
                Finish_Time = target.Finish_Time
            };
            return Response<TargetDTO>.Ok(result);
        }
        catch (DbUpdateException e)
        {
            return Response<TargetDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<TargetDTO>.Fail($"Error during create target: {e.Message}");
        }
    }

    //[HttpPut] methods
    public async Task<Response<TargetDTO>> UpdateTarget(int id, TargetMiniDTO dto)
    {
        try
        {
            var target = await _context.Targets
                .Include(t => t.Tag)
                .FirstOrDefaultAsync(e => e.ID == id);
            if (target == null)
            {
                return Response<TargetDTO>.Fail("Target not found");
            }
            
            var tag = await _context.Set<Tag>().FirstOrDefaultAsync(e => e.Name == dto.Tag);
            
            target.Name = dto.Name;
            target.Description = dto.Description;
            target.Start_Time = dto.Start_Time;
            target.Finish_Time = dto.Finish_Time;
            //target.Tag = tag;
            await _context.SaveChangesAsync();

            var newTarget = new TargetDTO
            {
                ID = target.ID,
                Name = target.Name,
                Description = target.Description,
                Start_Time = target.Start_Time,
                Finish_Time = target.Finish_Time,
                //Tag = target.Tag.Name
            };
            return Response<TargetDTO>.Ok(newTarget);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<TargetDTO>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<TargetDTO>.Fail($"Error during updating target: {e.Message}");
        }
    }

    //[HttpDelete] methods
    public async Task<Response<TargetDTO>> DeleteTarget(int targetId)
    {
        try
        {
            var target = await _context.Targets
                .Include(t => t.Users)
                .FirstOrDefaultAsync(e => e.ID == targetId);

            if (target == null)
                return Response<TargetDTO>.Fail("Target not found");

            target.Users.Clear();
            _context.Targets.Remove(target);
            await _context.SaveChangesAsync();

            return Response<TargetDTO>.Ok(new TargetDTO
            {
                ID = target.ID,
                Name = target.Name,
                Description = target.Description,
                Start_Time = target.Start_Time,
                Finish_Time = target.Finish_Time,
                //Tag = target.Tag.Name
            });
        }
        catch (DbUpdateException e)
        {
            return Response<TargetDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<TargetDTO>.Fail($"Error during deleting target: {e.Message}");
        }
    }
}