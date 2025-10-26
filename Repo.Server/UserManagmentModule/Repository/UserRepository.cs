using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
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
            .Where(u=>u.Deleted!=1)
            .ToListAsync();
    }

    public async Task<List<User>> GetAllUsersFromGroup(int groupId)
    {
        return await _context.Users
            .Include(u=>u.Groups)
            .Where(u=>u.Deleted!=1 && u.Groups.Any(g=>g.ID==groupId))
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

    public async Task<User> UpdateUser(User user)
    {
        var existingUser = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.ID == user.ID);

        if (existingUser == null)
            throw new InvalidOperationException($"User with ID {user.ID} not found");
        
        _context.Entry(existingUser).CurrentValues.SetValues(user);
        
        existingUser.Roles.Clear();
        foreach (var role in user.Roles)
        {
            existingUser.Roles.Add(role);
        }
        
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> DeleteUser(int id)
    {
        var user = _context.Set<User>().Find(id);
        user.Deleted = 1;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}