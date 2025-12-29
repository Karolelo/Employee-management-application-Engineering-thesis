using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;

namespace Repo.Server.GradeModule.Interfaces;

public interface IUserGradeService
{
    Task<Response<ICollection<UserMiniDTO>>> GetUsers(string? q, int page, int pageSize);
    Task<Response<UserMiniDTO>> GetUserById(int id);
}