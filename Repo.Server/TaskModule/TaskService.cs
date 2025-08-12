using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.task;
using Repo.Server.TaskModule.interafaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule;
public class TaskService : ITaskService
{
    private readonly MyDbContext _context;
    
    public TaskService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Response<ICollection<Task>>> GetUserTasks(int userId)
    {
        var result = await _context.Set<Task>()
            .Include(t => t.Users)
            .Where(e => e.Users.Any(u => u.ID == userId))
            .ToListAsync();
    
        return result.Count == 0 
            ? Response<ICollection<Task>>.Fail("User has no tasks") 
            : Response<ICollection<Task>>.Ok(result);
    }
    public async Task<Response<Task>> GetTaskById(int id)
    {
        var result = await _context.Set<Task>()
            .Include(t => t.RelatedTaskRelated_Tasks)
            .FirstOrDefaultAsync(e => e.ID == id);
        return result == null ? Response<Task>.Fail("Task not found") : Response<Task>.Ok(result);
    }

    public async Task<Response<ICollection<Tuple<Task, ICollection<Task>>>>> GetTaskWithRelatedTasks(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<ICollection<Task>>> GetGroupTasks(int groupdId)
    {
        var result = await _context.Set<Task>()
            .Include(t => t.Groups)  
            .Where(e => e.Groups.Any(g => g.ID == groupdId))
            .ToListAsync();
        
        return result.Count == 0 ? Response<ICollection<Task>>.Fail("User has no tasks") : Response<ICollection<Task>>.Ok(result);
    }

    public async Task<Response<Task>> CreateTask(CreateTaskModel model)
    {
        try
        {
            var task = new Task()
            {
                Name = model.Name,
                Description = model.Description,
                Start_Time = model.Start_Time,
                //Estimated_Time = model.Estimated_Time,
            };

            var priority = await _context.Set<Priority>().FirstOrDefaultAsync(e => e.Priority1 == model.Priority);

            if (priority == null)
            {
                return Response<Task>.Fail("Priority not found");
            }

            var status = await _context.Set<Status>().FirstOrDefaultAsync(e => e.Status1 == model.Status);
            if (status == null)
            {
                return Response<Task>.Fail("Status not found");
            }

            task.Priority = priority;
            task.Status = status;

            await _context.Set<Task>().AddAsync(task);
            await _context.SaveChangesAsync();
            return Response<Task>.Ok(task);
        }
        catch (Exception e)
        {
            return Response<Task>.Fail($"Error during creating task: {e.Message}");
        }
    }

    public async Task<Response<Task>> CreateTaskAssignToUser(CreateTaskModel model, int userId)
    {
        var user = _context.Set<User>().FirstOrDefault(e => e.ID == userId);
        if (user == null)
        {
            return Response<Task>.Fail("User not found");
        }
        
        var task = await CreateTask(model);
        if (!task.Success)
        {
            return Response<Task>.Fail(task.Error);
        }
        
        task.Data.Users.Add(user);
        await _context.SaveChangesAsync();
        return Response<Task>.Ok(task.Data);
    }

    public async Task<Response<Task>> CreateTaskAssignToGroup(CreateTaskModel model, int groupId)
    {
        var group = _context.Set<Group>().FirstOrDefault(e => e.ID == groupId);
        if (group == null)
        {
            return Response<Task>.Fail("Group not found");
        }
        
        var task = await CreateTask(model);
        if (!task.Success)
        {
            return Response<Task>.Fail(task.Error);
        }
        
        task.Data.Groups.Add(group);
        await _context.SaveChangesAsync();
        return Response<Task>.Ok(task.Data);
    }

    public async Task<Response<Task>> UpdateTask(UpdateTaskModel model, int id)
    {
        try
        {
            var task = await _context.Set<Task>().FirstOrDefaultAsync(e => e.ID == id);
            if (task == null)
            {
                return Response<Task>.Fail("Task not found");
            }

            task.Name = model.Name;
            task.Description = model.Description;
            task.Start_Time = model.Start_Time;
            //task.Estimated_Time = model.Estimated_Time;
            await _context.SaveChangesAsync();
            return Response<Task>.Ok(task);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<Task>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<Task>.Fail($"Error during updating task: {e.Message}");
        }
    }

    public Task<Response<Task>> UpdateTaskStatus(int id, int status)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Response<Task>> DeleteTask(int id)
    {
        try
        {
            var task = await _context.Set<Task>().FirstOrDefaultAsync(e => e.ID == id);

            if (task == null)
            {
                return Response<Task>.Fail("Task not found");
            }

            if (task.Deleted == 1)
            {
                return Response<Task>.Fail("Task already deleted");
            }
            task.Deleted = 1;
            _context.Set<Task>().Update(task);
            await _context.SaveChangesAsync();
            return Response<Task>.Ok(task);
        }
        catch (Exception e)
        {
            return Response<Task>.Fail($"Error during updating task: {e.Message}");
        }
    }
}