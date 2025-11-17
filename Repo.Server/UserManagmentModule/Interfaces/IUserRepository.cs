using Repo.Core.Models;
using Repo.Core.Models.auth;
using Repo.Core.Models.user;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<List<User>> GetAllUsersFromGroup(int groupId);
    Task<User?> GetUserById(int id);
    //This method I created for validation of user
    //For example to not create same user x time
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByNickname(string nickname);
    //Registration we handle in authModule
    //Because of addintional logic included in creating user like salting password etc ...
    /*Task<bool> CreateUser(RegistrationModel model);*/
    Task<User> UpdateUser(User user);
    Task<bool> DeleteUser(int id);
}