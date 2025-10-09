using Repo.Core.Models;
using Repo.Core.Models.auth;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(int id);
    //Registration we handle in authModule
    /*Task<bool> CreateUser(RegistrationModel model);*/
    Task<bool> UpdateUser(User user);
    Task<bool> DeleteUser(int id);
    
    //Group managment
    Task<List<Group>> GetAllGroups();
    Task<Group> GetGroupById(int id);
    Task<bool> CreateGroup(Group group);
    Task<bool> UpdateGroup(Group group);
    Task<bool> DeleteGroup(int id);
    Task<bool> AddUserToGroup(int userId, int groupId);
    Task<bool> RemoveUserFromGroup(int userId, int groupId);
    Task<bool> SetLeaderOfGroup(int userId, int groupId);
}