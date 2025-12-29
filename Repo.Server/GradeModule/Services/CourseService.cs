using Microsoft.EntityFrameworkCore;
using Repo.Core.Extensions;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
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
            .Select(c => new CourseDTO
            {
                ID = c.ID, 
                Name = c.Name,
                Description = c.Description,
                Start_Date = c.Start_Date,
                Finish_Date = c.Finish_Date
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<CourseDTO>>.Fail("No courses found")
            : Response<ICollection<CourseDTO>>.Ok(data);
    }

    public async Task<Response<CourseDTO>> GetCourseById(int id)
    {
        var result = await _context.Courses.AsNoTracking()
            .Where(c => c.ID == id)
            .Select(c => new CourseDTO {
                ID = c.ID, 
                Name = c.Name,
                Description = c.Description,
                Start_Date = c.Start_Date,
                Finish_Date = c.Finish_Date
            })
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
        try
        {
            var name = (dto.Name ?? string.Empty).Trim();
            var desc = (dto.Description ?? string.Empty).Trim();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                return Response<CourseDTO>.Fail("Name is required");
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                return Response<CourseDTO>.Fail("Description is required");
            }

            if (dto.Finish_Date < dto.Start_Date)
            {
                return Response<CourseDTO>.Fail("Finish date cannot be earlier than start date");
            }

            var exists = await _context.Courses.AsNoTracking().AnyAsync(c => c.Name == name);
            if (exists)
            {
                return Response<CourseDTO>.Fail("Course with this name already exists");
            }

            var course = new Course
            {
                Name = name,
                Description = desc,
                Start_Date = dto.Start_Date,
                Finish_Date = dto.Finish_Date,
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = new CourseDTO
            {
                ID = course.ID,
                Name = course.Name,
                Description = course.Description,
                Start_Date = course.Start_Date,
                Finish_Date = course.Finish_Date,
            };
            return Response<CourseDTO>.Ok(result);
        }
        catch (DbUpdateException e)
        {
            return Response<CourseDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<CourseDTO>.Fail($"Error during creating course: {e.Message}");
        }
    }

    public async Task<Response<object>> EnrollUser(int courseId, int userId)
    {
        try
        {
            var user = await _context.Users.Include(u => u.Courses).FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
            {
                return Response<object>.Fail("User not found");
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return Response<object>.Fail("Course not found");
            }

            if (user.Courses.Any(c => c.ID == courseId))
            {
                return Response<object>.Fail("User already enrolled");
            }

            user.Courses.Add(course);
            await _context.SaveChangesAsync();
            return Response<object>.Ok(new { enrolled = true });
        }
        catch (DbUpdateException e) when (e.IsUniqueViolation())
        {
            return Response<object>.Fail("User already enrolled");
        }
        catch (DbUpdateException e)
        {
            return Response<object>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<object>.Fail($"Error during enrolling user: {e.Message}");
        }
    }

    //[HttpPut] methods
    public async Task<Response<CourseDTO>> UpdateCourse(int courseId, CourseMiniDTO dto)
    {
        try
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.ID == courseId);
            if (course == null)
            {
                return Response<CourseDTO>.Fail("Course not found");
            }

            var name = (dto.Name ?? string.Empty).Trim();
            var desc = (dto.Description ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return Response<CourseDTO>.Fail("Name is required");
            }

            if (string.IsNullOrWhiteSpace(desc))
            {
                return Response<CourseDTO>.Fail("Description is required");
            }

            if (dto.Finish_Date < dto.Start_Date)
            {
                return Response<CourseDTO>.Fail("Finish date cannot be earlier than start date");
            }

            var exists = await _context.Courses.AsNoTracking().AnyAsync(c => c.ID != courseId && c.Name == dto.Name);
            if (exists)
            {
                return Response<CourseDTO>.Fail("Course with this name already exists");
            }

            course.Name = name;
            course.Description = desc;
            course.Start_Date = dto.Start_Date;
            course.Finish_Date = dto.Finish_Date;

            await _context.SaveChangesAsync();

            var result = new CourseDTO
            {
                ID = course.ID,
                Name = course.Name,
                Description = course.Description,
                Start_Date = course.Start_Date,
                Finish_Date = course.Finish_Date,
            };

            return Response<CourseDTO>.Ok(result);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<CourseDTO>.Fail("Concurrency conflict");
        }
        catch (DbUpdateException e)
        {
            return Response<CourseDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<CourseDTO>.Fail($"Error during updating course: {e.Message}");
        }
    }

    //[HttpDelete] methods
    public async Task<Response<CourseDTO>> DeleteCourse(int courseId)
    {
        try
        {
            var course = await _context.Courses
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.ID == courseId);
            if (course == null)
            {
                return Response<CourseDTO>.Fail("Course not found");
            }

            if (course.Users.Count > 0)
            {
                return Response<CourseDTO>.Fail(
                    $"Course has {course.Users.Count} participant(s). Unenroll them first");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Response<CourseDTO>.Ok(new CourseDTO
            {
                ID = course.ID,
                Name = course.Name,
                Description = course.Description,
                Start_Date = course.Start_Date,
                Finish_Date = course.Finish_Date
            });
        }
        catch (DbUpdateException e) when (e.IsUniqueViolation())
        {
            return Response<CourseDTO>.Fail("Course has participants. Unenroll them first");
        }
        catch (DbUpdateException e)
        {
            return Response<CourseDTO>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<CourseDTO>.Fail($"Error during deleting course: {e.Message}");
        }
    }

    public async Task<Response<object>> UnenrollUser(int courseId, int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Courses)
                .FirstOrDefaultAsync(u => u.ID == userId);
            if (user == null)
                return Response<object>.Fail("User not found");

            var course = user.Courses.FirstOrDefault(c => c.ID == courseId);
            if (course == null)
                return Response<object>.Fail("Enrollment not found");

            user.Courses.Remove(course);
            await _context.SaveChangesAsync();
            
            return Response<object>.Ok(new { enrolled = false });
        }
        catch (DbUpdateException e)
        {
            return Response<object>.Fail($"DB error: {e.Message}");
        }
        catch (Exception e)
        {
            return Response<object>.Fail($"Error during unenrolling user: {e.Message}");
        }
    }
}