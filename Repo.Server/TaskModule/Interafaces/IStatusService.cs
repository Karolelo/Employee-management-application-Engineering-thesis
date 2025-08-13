using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;

namespace Repo.Server.TaskModule.interafaces;

public interface IStatusService
{
    Task<Response<Status>> AddStatus(StatusDTO status);
    Task<Response<StatusDTO>> UpdateStatus(StatusDTO status, int id);
    Task<Response<StatusDTO>> GetStatusById(int id);
}