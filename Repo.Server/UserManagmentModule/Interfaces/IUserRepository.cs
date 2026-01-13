using Repo.Core.Models;
using Repo.Core.Models.auth;
using Repo.Core.Models.user;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExists(int userId);
    Task<List<User>> GetAllUsers();
    Task<List<User>> GetAllUsersFromGroup(int groupId);
    Task<User?> GetUserById(int id);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByNickname(string nickname);
    Task<User?> GetUserByLogin(string login);
    Task<User> CreateUser(User user);
    Task<User> UpdateUser(User user);
    Task<bool> DeleteUser(int id);
    Task<List<string>> GetUserRoles(int userId);
    bool IsAdminHasGroup(int adminId);
    Task<bool> AddTaskToUser(int userId, Core.Models.Task task);
    Task<bool> AddTaskToUser(int userId, int taskId);
}