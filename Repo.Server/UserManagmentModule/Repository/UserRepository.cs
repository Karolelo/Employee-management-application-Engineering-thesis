using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.auth;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Repository;

public class UserRepository : IUserRepository
{
    private readonly MyDbContext _context;
    
    public UserRepository(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users
            .Include(u=>u.Roles)
            .Include(u => u.Groups)
            .ToListAsync();
    }

    public async Task<List<User>> GetAllUsersFromGroup(int groupId)
    {
        return await _context.Users
            .Include(u=>u.Groups)
            .Where(u=> u.Groups.Any(g=>g.ID==groupId))
            .ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users
            .Include(u=>u.Roles)
            .FirstOrDefaultAsync(u => u.ID == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetUserByNickname(string nickname)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Nickname.ToLower() == nickname.ToLower());
    }
    
    public async Task<User?> GetUserByLogin(string login)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login.ToLower() ==login.ToLower());
    }

    public async Task<User> CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUser(User user)
    {
        _context.Entry(user).CurrentValues.SetValues(user);
        
        user.Roles.Clear();
        foreach (var role in user.Roles)
        {
            user.Roles.Add(role);
        }
        
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        user.Deleted = 1;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<List<string>> GetUserRoles(int userId)
    {
        var roles = await _context.Users.Include(u=>u.Roles)
            .Where(u => u.ID == userId)
            .SelectMany(u => u.Roles)
            .Select(r => r.Role_Name)
            .ToListAsync();
        //I add basic roles
        return roles.Count > 0 ? roles : new List<string>(){"User"};
    }

    public bool IsAdminHasGroup(int adminId)
    {
        return _context.Groups.Any(g => g.Admin_ID == adminId);
    }
}