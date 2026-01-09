using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.TaskModule.interafaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule.Repository;

public class TaskRepository(MyDbContext context) : ITaskRepository
{
    private readonly MyDbContext _context = context;

    public async Task<Task?> GetTaskById(int id)
    {
        return await _context.Tasks
            .Include(t=>t.Priority)
            .Include(t=>t.Status)
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<IEnumerable<Task>> GetUserTasks(int userId)
    {
        
        var result =  await _context.Tasks
            .Include(t => t.Users)
            .Include(t=>t.Priority)
            .Include(t=>t.Status)
            .Where(t => t.Users.Any(u => u.ID == userId))
            .ToListAsync();
        Debug.WriteLine(result);
        return result;
    }

    public async Task<IEnumerable<Task>> GetGroupTasks(int groupId)
    {
        return await _context.Tasks
            .Include(t => t.Groups)
            .Include(t=>t.Priority)
            .Include(t=>t.Status)
            .Where(t => t.Groups.Any(u => u.ID == groupId))
            .ToListAsync();
    }

    public async Task<Task> CreateTask(Task task)
    {
        var result = await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Task> UpdateTask(Task task)
    {
        var result = await _context.Tasks
            .Where(t => t.ID == task.ID)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.Name, task.Name)
                .SetProperty(t => t.Description, task.Description)
                .SetProperty(t => t.Start_Time, task.Start_Time)
                .SetProperty(t => t.Estimated_Time, task.Estimated_Time)
                .SetProperty(t => t.Priority_ID, task.Priority_ID)
                .SetProperty(t => t.Status_ID, task.Status_ID)
            );
        
        return task;
    }

    public async Task<bool> DeleteTask(int id)
    {
        var result = await _context.Tasks
            .Where(t => t.ID == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(t => t.Deleted, 1));
        return result > 0;
    }

    public async Task<RelatedTask> AddRelation(int taskId, int relatedTaskId)
    {
        var result = await _context.RelatedTasks.AddAsync(new RelatedTask
        {
            Main_Task_ID = taskId,
            Related_Task_ID = relatedTaskId
        });
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<bool> RemoveRelation(int taskId, int relatedTaskId)
    {
        var result = await _context.RelatedTasks
            .Where(rt => 
                (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId))
            .ExecuteDeleteAsync();
        return result > 0;
    }

    public async Task<List<int>> GetRelatedTaskIds(int taskId)
    {
        var relatedIds = await _context.RelatedTasks
            .AsNoTracking() 
            .Where(rt => rt.Main_Task_ID == taskId || rt.Related_Task_ID == taskId)
            .Select(rt => rt.Main_Task_ID == taskId ? rt.Related_Task_ID : rt.Main_Task_ID)
            .Distinct()
            .ToListAsync();
        return relatedIds;
    }

    public async Task<bool> RelationExists(int taskId, int relatedTaskId)
    {
        return await _context.RelatedTasks
            .AnyAsync(rt =>
                (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId));
    }

    public async Task<bool> TaskExists(int taskId)
    {
        return await _context.Tasks.AnyAsync(t => t.ID == taskId);
    }
}