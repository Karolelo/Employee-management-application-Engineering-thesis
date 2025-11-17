using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Task = Repo.Core.Models.Task;
namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IUserService
{
    Task<Response<List<UserDTO>>> GetAllUsers();

    Task<Response<List<UserDTO>>> GetAllUsersFromGroup(int groupId);

    Task<Response<List<UserDTO>>> GetTeamLeadersWithoutGroup();
    
    Task<Response<UserDTO>> GetUserById(int id);

    Task<Response<List<UserDTO>>> GetUsersWithRole(string role);
    
    Task<Response<UserDTO>> UpdateUser(UserUpdateDTO dto);
    
    Task<Response<bool>> DeleteUser(int id);
}