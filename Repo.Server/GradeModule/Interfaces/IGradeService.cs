using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;

namespace Repo.Server.GradeModule.Interfaces;

public interface IGradeService
{
    //[HttpGet] methods
    Task<Response<ICollection<GradeDTO>>> GetGrades(string? q, int page, int pageSize);
    Task<Response<GradeDTO>> GetGradeById(int id);
    Task<Response<ICollection<GradeDTO>>> GetUserGrades(int userId);
    
    //[HttpPost] methods
    Task<Response<GradeDTO>> CreateGrade(int userId, GradeMiniDTO dto);

    //[HttpPut] methods
    Task<Response<GradeDTO>> UpdateGrade(int gradeId, GradeMiniDTO dto);

    //[HttpDelete] methods
    Task<Response<GradeDTO>> DeleteGrade(int gradeId);
}