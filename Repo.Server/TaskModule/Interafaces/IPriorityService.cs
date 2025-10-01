using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;

namespace Repo.Server.TaskModule.interafaces;

public interface IPriorityService
{
    //Methods for getting priority
    Task<Response<PriorityDTO>> GetPriorityById(int id);
    Task<Response<IEnumerable<PriorityDTO>>> GetAllPriority();
    Task <Response<IEnumerable<PriorityDTO>>>  GetPriorityByName(string name);
    
    //Methods for creating priority
    Task<Response<Priority>> AddPriority(PriorityDTO priority);
    
    //Methods for updating priority
    Task<Response<PriorityDTO>> UpdatePriority(PriorityDTO priority, int id);
}