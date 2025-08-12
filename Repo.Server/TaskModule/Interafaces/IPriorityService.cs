using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;

namespace Repo.Server.TaskModule.interafaces;

public interface IPriorityService
{
    Task<Response<Priority>> AddPriority(PriorityDTO priority);
    Task<Response<PriorityDTO>> UpdatePriority(PriorityDTO priority, int id);
    Task<Response<Priority>> GetPriorityById(int id);
}