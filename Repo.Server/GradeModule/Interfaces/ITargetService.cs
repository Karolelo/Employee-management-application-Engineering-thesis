using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;

namespace Repo.Server.GradeModule.Interfaces;

public interface ITargetService
{
    //[HttpGet] methods
    Task<Response<ICollection<TargetDTO>>> GetTargets(string? q, int page, int pageSize);
    Task<Response<TargetDTO>> GetTargetById(int id);
    Task<Response<ICollection<TargetDTO>>> GetUserTargets(int userId);
    
    //[HttpPost] methods
    Task<Response<TargetDTO>> CreateTarget(int userId, TargetMiniDTO dto);
    
    //[HttpPut] methods
    Task<Response<TargetDTO>> UpdateTarget(int id, TargetMiniDTO dto);
    
    //[HttpDelete] methods
    Task<Response<TargetDTO>> DeleteTarget(int targetId);
}