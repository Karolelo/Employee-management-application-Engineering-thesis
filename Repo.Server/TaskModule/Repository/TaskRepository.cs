using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.TaskModule.interafaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule.Repository;

public class TaskRepository(MyDbContext context) : ITaskRepository
{

    public async Task<Task?> GetTaskById(int id)
    {
        return await context.Tasks
            .Include(t=>t.Priority)
            .Include(t=>t.Status)
            .FirstOrDefaultAsync(x => x.ID == id);
    }

    public async Task<IEnumerable<Task>> GetUserTasks(int userId)
    {
        
        var result =  await context.Tasks
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
        return await context.Tasks
            .Include(t => t.Groups)
            .Include(t=>t.Priority)
            .Include(t=>t.Status)
            .Where(t => t.Groups.Any(u => u.ID == groupId))
            .ToListAsync();
    }

    public async Task<Task> CreateTask(Task task)
    {
        var result = await context.Tasks.AddAsync(task);
        await context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Task> UpdateTask(Task task)
    {
        var result = await context.Tasks
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
        var result = await context.Tasks
            .Where(t => t.ID == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(t => t.Deleted, 1));
        return result > 0;
    }

    public async Task<RelatedTask> AddRelation(int taskId, int relatedTaskId)
    {
        var result = await context.RelatedTasks.AddAsync(new RelatedTask
        {
            Main_Task_ID = taskId,
            Related_Task_ID = relatedTaskId
        });
        await context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<bool> RemoveRelation(int taskId, int relatedTaskId)
    {
        var result = await context.RelatedTasks
            .Where(rt => 
                (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId))
            .ExecuteDeleteAsync();
        return result > 0;
    }

    public async Task<List<int>> GetRelatedTaskIds(int taskId)
    {
        var relatedIds = await context.RelatedTasks
            .AsNoTracking() 
            .Where(rt => rt.Main_Task_ID == taskId || rt.Related_Task_ID == taskId)
            .Select(rt => rt.Main_Task_ID == taskId ? rt.Related_Task_ID : rt.Main_Task_ID)
            .Distinct()
            .ToListAsync();
        return relatedIds;
    }

    public async Task<bool> RelationExists(int taskId, int relatedTaskId)
    {
        return await context.RelatedTasks
            .AnyAsync(rt =>
                (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId));
    }

    public async Task<bool> TaskExists(int taskId)
    {
        return await context.Tasks.AnyAsync(t => t.ID == taskId);
    }
    
    public async Task<IEnumerable<Core.Models.Task>> GetUserTasksForGantt(int userId)
    {
        return await context.Tasks
            .AsNoTracking()
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Users)
            .Where(t => t.Users.Any(u => u.ID == userId) && t.Deleted == 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<RelatedTask>> GetRelatedTasksByMainTaskIds(List<int> taskIds)
    {
        return await context.RelatedTasks
            .AsNoTracking()
            .Where(rt => taskIds.Contains(rt.Main_Task_ID))
            .ToListAsync();
    }
}