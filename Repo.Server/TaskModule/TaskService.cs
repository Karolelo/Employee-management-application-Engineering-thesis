using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Server.TaskModule.interafaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule;

public class TaskService : ITaskManager
{
    private readonly MyDbContext _context;
    
    public TaskService(MyDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<ICollection<Task>> GetUserTasks(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        
        var isValidUser = await _context.Set<User>().AnyAsync(u => u.ID == user.ID);
        if (!isValidUser)
        {
            throw new ArgumentException("Provided user is invalid or does not exist in the database.");
        }

        var respond = await _context.Set<Task>()
            .Where(e => e.Users.Contains(user) && e.Status.Status1 == "active")
            .ToListAsync();

        if (respond == null || respond.Count == 0)
        {
            throw new InvalidOperationException("No active tasks found for the given user.");
        }

        return respond;
    }

    public Task<Task> GetTaskById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Task>> GetGroupTask(int status)
    {
        throw new NotImplementedException();
    }

    public Task<Task> CreateTask(Task task)
    {
        throw new NotImplementedException();
    }

    public Task<Task> CreateTaskAssignToUser(Task task, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Task> CreateTaskAssignToGroup(Task task, int groupId)
    {
        throw new NotImplementedException();
    }

    public Task<Task> UpdateTask(Task task)
    {
        throw new NotImplementedException();
    }

    public Task<Task> UpdateTaskStatus(int id, int status)
    {
        throw new NotImplementedException();
    }

    public void DeleteTask(int id)
    {
        throw new NotImplementedException();
    }
}