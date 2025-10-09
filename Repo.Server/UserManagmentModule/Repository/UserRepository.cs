using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Repository;

public class UserRepository : IUserRepository
{
    private readonly MyDbContext _context;
    
    public UserRepository(MyDbContext context)
    {
        _context = context;
    }
    
    public Task<List<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(int id)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Group>> GetAllGroups()
    {
        throw new NotImplementedException();
    }

    public Task<Group> GetGroupById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateGroup(Group group)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateGroup(Group group)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteGroup(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserToGroup(int userId, int groupId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveUserFromGroup(int userId, int groupId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetLeaderOfGroup(int userId, int groupId)
    {
        throw new NotImplementedException();
    }
}