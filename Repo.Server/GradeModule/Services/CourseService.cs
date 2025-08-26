using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Services;

public class CourseService : ICourseService
{
    private readonly MyDbContext _context;

    public CourseService(MyDbContext context)
    {
        _context = context;
    }

    //[HttpGet] methods
    public async Task<Response<ICollection<CourseDTO>>> GetCourses(string? q, int page, int pageSize)
    {
        var query = _context.Courses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(c => c.Name.Contains(q));
        }

        var data = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CourseDTO{ID = c.ID, Name = c.Name})
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<CourseDTO>>.Fail("No courses found")
            : Response<ICollection<CourseDTO>>.Ok(data);
    }

    public async Task<Response<CourseDTO>> GetCourseById(int id)
    {
        var result = await _context.Courses.AsNoTracking()
            .Where(c => c.ID == id)
            .Select(c => new CourseDTO { ID = c.ID, Name = c.Name })
            .FirstOrDefaultAsync();

        return result == null
            ? Response<CourseDTO>.Fail("Course not found")
            : Response<CourseDTO>.Ok(result);
    }

    public async Task<Response<ICollection<UserMiniDTO>>> GetParticipants(int courseId)
    {
        try
        {
            var courseExists = await _context.Courses
                .AsNoTracking()
                .AnyAsync(c => c.ID == courseId);
            if (!courseExists)
                return Response<ICollection<UserMiniDTO>>.Fail("Course not found");
            
            var result = await _context.Courses.AsNoTracking()
                .Where(c => c.ID == courseId)
                .SelectMany(c => c.Users)
                .Select(u => new UserMiniDTO { ID = u.ID, Login = u.Login, Nickname = u.Nickname })
                .ToListAsync();

            return result.Count == 0
                ? Response<ICollection<UserMiniDTO>>.Fail("No participants found")
                : Response<ICollection<UserMiniDTO>>.Ok(result);
        }
        catch (Exception e)
        {
            return Response<ICollection<UserMiniDTO>>.Fail($"Error during searching: {e.Message}");
        }
    }

    public async Task<Response<ICollection<CourseDTO>>> GetUserCourses(int userId)
    {
        try
        {
            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.ID == userId);
            if (!userExists)
                return Response<ICollection<CourseDTO>>.Fail("User not found");
            
            var courses = await _context.Users
                .AsNoTracking()
                .Where(u => u.ID == userId)
                .SelectMany(u => u.Courses)
                .Select(c => new CourseDTO { ID = c.ID, Name = c.Name })
                .ToListAsync();
            
            return courses.Count == 0
                ? Response<ICollection<CourseDTO>>.Fail("No courses found")
                : Response<ICollection<CourseDTO>>.Ok(courses);
        }
        catch (Exception e)
        {
            return Response<ICollection<CourseDTO>>.Fail($"Error during searching: {e.Message}");
        }
    }

    //[HttpPost] methods
    public async Task<Response<CourseDTO>> CreateCourse(CourseMiniDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<object>> EnrollUser(int courseId, int userId)
    {
        throw new NotImplementedException();
    }

    //[HttpPut] methods
    public async Task<Response<object>> UpdateCourse(int courseId, CourseMiniDTO dto)
    {
        throw new NotImplementedException();
    }

    //[HttpDelete] methods
    public async Task<Response<object>> DeleteCourse(int courseId)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<object>> UnenrollUser(int courseId, int userId)
    {
        throw new NotImplementedException();
    }
}