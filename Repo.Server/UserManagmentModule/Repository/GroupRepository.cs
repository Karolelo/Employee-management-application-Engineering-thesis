using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Repository;

public class GroupRepository : IGroupRepository
{
    private readonly MyDbContext _context;
    
    public GroupRepository(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Group>> GetAllGroups()
    {
        return await _context.Set<Group>().ToListAsync();
    }

    public async Task<Group?> GetGroupById(int id)
    {
        return await _context.Set<Group>().FirstOrDefaultAsync(g => g.ID == id);
    }

    public async Task<Group> CreateGroup(Group group)
    {
        await _context.Set<Group>().AddAsync(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<Group> UpdateGroup(Group group)
    {
        _context.Set<Group>().Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        var group = await _context.Set<Group>().FindAsync(id);
        _context.Set<Group>().Remove(group);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> AddUserToGroup(int userId, int groupId)
    {
        var user = _context.Users.Find(userId);
        var group = _context.Groups.Include(g => g.Users).First(g => g.ID == groupId);
        group.Users.Add(user);
        var result = await _context.SaveChangesAsync();
        
        return result > 0;
    }

    public async Task<bool> RemoveUserFromGroup(int userId, int groupId)
    {
        var user = _context.Users.Find(userId);
        var group = _context.Groups.Include(g => g.Users).First(g => g.ID == groupId);
        group.Users.Remove(user);
        var result = await _context.SaveChangesAsync();
        
        return result > 0;
    }

    public async Task<bool> SetLeaderOfGroup(int userId, int groupId)
    {
        var group = _context.Groups.Find(groupId);
        group.Admin_ID = userId;
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }
}