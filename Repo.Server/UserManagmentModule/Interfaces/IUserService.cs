using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Task = Repo.Core.Models.Task;
namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IUserService
{
    Task<Response<List<UserDto>>> GetAllUsers();
    
    Task<Response<UserDto>> GetUserById(int id);
    
    Task<Response<UserDto>> UpdateUser(UserUpdateDTO dto);
    
    Task<Response<bool>> DeleteUser(int id);
}