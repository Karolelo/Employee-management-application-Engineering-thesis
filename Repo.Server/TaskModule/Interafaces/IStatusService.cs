using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;

namespace Repo.Server.TaskModule.interafaces;

public interface IStatusService
{
    //Methods for getting status
    Task<Response<StatusDTO>> GetStatusById(int id);
    
    //Methods for creating status
    Task<Response<Status>> AddStatus(StatusDTO status);
    
    //Methods for updating status
    Task<Response<StatusDTO>> UpdateStatus(StatusDTO status, int id);
}