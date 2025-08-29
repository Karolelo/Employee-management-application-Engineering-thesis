using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Services;

public class GradeService : IGradeService
{
    private readonly MyDbContext _context;

    public GradeService(MyDbContext context)
    {
        _context = context;
    }
    
    //[HttpGet] methods
    public async Task<Response<ICollection<GradeDTO>>> GetGrades(string? q, int page, int pageSize)
    {
        var query = _context.Grades.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(g => g.Description.Contains(q));

        var data = await query
            .OrderByDescending(g => g.Start_Date)
            .ThenByDescending(g => g.Finish_Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new GradeDTO
            {
                ID = g.ID,
                Grade = g.Grade1,
                Description = g.Description,
                Start_Date = g.Start_Date,
                Finish_Date = g.Finish_Date
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<GradeDTO>>.Fail("No grades found")
            : Response<ICollection<GradeDTO>>.Ok(data);
    }
    
    public async Task<Response<GradeDTO>> GetGradeById(int id)
    {
        var result = await _context.Grades.AsNoTracking()
            .Where(x => x.ID == id)
            .Select(x => new GradeDTO
            {
                ID = x.ID,
                Grade = x.Grade1,
                Description = x.Description,
                Start_Date = x.Start_Date,
                Finish_Date = x.Finish_Date
            })
            .FirstOrDefaultAsync();

        return result == null ? Response<GradeDTO>.Fail("Grade not found") : Response<GradeDTO>.Ok(result);
    }
    
    public async Task<Response<ICollection<GradeDTO>>> GetUserGrades(int userId)
    {
        var exists = await _context.Users.AsNoTracking().AnyAsync(u => u.ID == userId);
        if (!exists) return Response<ICollection<GradeDTO>>.Fail("User not found");

        var data = await _context.Users.AsNoTracking()
            .Where(u => u.ID == userId)
            .SelectMany(u => u.Grades)
            .Select(g => new GradeDTO
            {
                ID = g.ID,
                Grade = g.Grade1,
                Description = g.Description,
                Start_Date = g.Start_Date,
                Finish_Date = g.Finish_Date
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<GradeDTO>>.Fail("No grades found")
            : Response<ICollection<GradeDTO>>.Ok(data);
    }
    
    //[HttpPost] methods
    public async Task<Response<GradeDTO>> CreateGrade(int userId, GradeMiniDTO dto)
    {
        try
        {
            var user = await _context.Users.Include(u => u.Grades).FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null) return Response<GradeDTO>.Fail("User not found");

            if (dto.Finish_Date < dto.Start_Date)
                return Response<GradeDTO>.Fail("Finish date cannot be earlier than start date");

            // Unique per user: (Grade1, Start, Finish)
            var duplicateForUser = await _context.Grades.AsNoTracking()
                .Where(g => g.Grade1 == dto.Grade
                            && g.Start_Date == dto.Start_Date
                            && g.Finish_Date == dto.Finish_Date
                            && g.Users.Any(u => u.ID == userId))
                .AnyAsync();

            if (duplicateForUser)
                return Response<GradeDTO>.Fail("User already has a grade for that period with the same value");

            var grade = new Grade
            {
                Grade1 = dto.Grade,
                Description = dto.Description.Trim(),
                Start_Date = dto.Start_Date,
                Finish_Date = dto.Finish_Date
            };

            grade.Users.Add(user);
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            var result = new GradeDTO
            {
                ID = grade.ID,
                Grade = grade.Grade1,
                Description = grade.Description,
                Start_Date = grade.Start_Date,
                Finish_Date = grade.Finish_Date
            };
            return Response<GradeDTO>.Ok(result);
        }
        catch (DbUpdateException e)
        {
            return Response<GradeDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<GradeDTO>.Fail($"Error during creating grade: {e.Message}");
        }
    }
    
    //[HttpPut] methods
    public async Task<Response<GradeDTO>> UpdateGrade(int id, GradeMiniDTO dto)
    {
        try
        {
            var grade = await _context.Grades
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.ID == id);

            if (grade == null) return Response<GradeDTO>.Fail("Grade not found");

            if (dto.Finish_Date < dto.Start_Date)
                return Response<GradeDTO>.Fail("Finish date cannot be earlier than start date");

            if (grade.Users.Count > 0)
            {
                var userIds = grade.Users.Select(u => u.ID).ToList();

                var conflict = await _context.Grades.AsNoTracking()
                    .Where(g => g.ID != id
                                && g.Grade1 == dto.Grade
                                && g.Start_Date == dto.Start_Date
                                && g.Finish_Date == dto.Finish_Date
                                && g.Users.Any(u => userIds.Contains(u.ID)))
                    .AnyAsync();

                if (conflict)
                    return Response<GradeDTO>.Fail("Update would create a duplicate grade for an assigned user");
            }

            grade.Grade1 = dto.Grade;
            grade.Description = dto.Description.Trim();
            grade.Start_Date = dto.Start_Date;
            grade.Finish_Date = dto.Finish_Date;

            await _context.SaveChangesAsync();

            var result = new GradeDTO
            {
                ID = grade.ID,
                Grade = grade.Grade1,
                Description = grade.Description,
                Start_Date = grade.Start_Date,
                Finish_Date = grade.Finish_Date
            };
            return Response<GradeDTO>.Ok(result);
        }
        catch (DbUpdateException e)
        {
            return Response<GradeDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<GradeDTO>.Fail($"Error during updating grade: {e.Message}");
        }
    }
    
    //[HttpDelete] methods
    public async Task<Response<GradeDTO>> DeleteGrade(int id)
    {
        try
        {
            var grade = await _context.Grades
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.ID == id);

            if (grade == null) return Response<GradeDTO>.Fail("Grade not found");

            grade.Users.Clear();
            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return Response<GradeDTO>.Ok(new GradeDTO
            {
                ID = grade.ID,
                Grade = grade.Grade1,
                Description = grade.Description,
                Start_Date = grade.Start_Date,
                Finish_Date = grade.Finish_Date
            });
        }
        catch (DbUpdateException e)
        {
            return Response<GradeDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<GradeDTO>.Fail($"Error during deleting grade: {e.Message}");
        }
    }
}