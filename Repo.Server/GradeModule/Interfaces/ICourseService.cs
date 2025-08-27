using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;

namespace Repo.Server.GradeModule.Interfaces;

public interface ICourseService
{
    //[HttpGet] methods
    Task<Response<ICollection<CourseDTO>>> GetCourses(string? q, int page, int pageSize);
    Task<Response<CourseDTO>> GetCourseById(int id);
    Task<Response<ICollection<UserMiniDTO>>> GetParticipants(int courseId);
    Task<Response<ICollection<CourseDTO>>> GetUserCourses(int userId);
    
    //[HttpPost] methods
    Task<Response<CourseDTO>> CreateCourse(CourseMiniDTO dto);
    Task<Response<object>> EnrollUser(int courseId, int userId);

    //[HttpPut] methods
    Task<Response<CourseDTO>> UpdateCourse(int courseId, CourseMiniDTO dto);

    //[HttpDelete] methods
    Task<Response<CourseDTO>> DeleteCourse(int courseId);
    Task<Response<object>> UnenrollUser(int courseId, int userId);
}