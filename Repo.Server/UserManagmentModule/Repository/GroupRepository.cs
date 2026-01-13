
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = System.Threading.Tasks.Task;

public class GroupRepository : IGroupRepository
{
    private readonly MyDbContext _context;
    private const string EmptyImagePath = "";

    public GroupRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Group>> GetAllGroups()
    {
        return await _context.Groups
            .Include(g => g.Users)
            .ToListAsync();
    }

    public async Task<Group?> GetGroupById(int id)
    {
        return await _context.Groups.FirstOrDefaultAsync(g => g.ID == id);
    }

    public async Task<Group> CreateGroup(Group group)
    {
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<Group> UpdateGroup(Group group)
    {
        _context.Groups.Attach(group);
        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<bool> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return false;
        
        group.Deleted = 1;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> AddUserToGroup(int userId, int groupId)
    {
        var user = await _context.Users.FindAsync(userId);
        var group = await GetGroupWithUsersAsync(groupId);
        
        if (user == null || group == null) return false;
        
        group.Users.Add(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> RemoveUserFromGroup(int userId, int groupId)
    {
        var user = await _context.Users.FindAsync(userId);
        var group = await GetGroupWithUsersAsync(groupId);
        
        if (user == null || group == null) return false;
        
        group.Users.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> AddTaskToGroup(int groupId, int taskId)
    {
        var group = await _context.Groups
            .Include(g => g.Tasks)
            .FirstOrDefaultAsync(g => g.ID == groupId);

        var task = await _context.Tasks.FindAsync(taskId);
        
        if (group == null || task == null) return false;
        
        group.Tasks.Add(task);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SetLeaderOfGroup(int userId, int groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null) return false;
        
        group.Admin_ID = userId;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<string> GetPathToImageFile(int groupId)
    {
        var groupImage = await _context.GroupImages.FirstOrDefaultAsync(g => g.GROUP_ID == groupId);
        return groupImage?.Path ?? EmptyImagePath;
    }

    public async Task<string> SavePathToImageFile(int groupId, string path)
    {
        var groupImage = new GroupImage()
        {
            GROUP_ID = groupId,
            Path = path
        };
        await _context.GroupImages.AddAsync(groupImage);
        await _context.SaveChangesAsync();
        return path;
    }

    public async Task<string> UpdateImageFile(int groupId, string path)
    {
        var groupImage = await _context.GroupImages.FindAsync(groupId);
        if (groupImage == null) return EmptyImagePath;
        
        groupImage.Path = path;
        await _context.SaveChangesAsync();
        return path;
    }

    private async Task<Group?> GetGroupWithUsersAsync(int groupId)
    {
        return await _context.Groups
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.ID == groupId);
    }
}