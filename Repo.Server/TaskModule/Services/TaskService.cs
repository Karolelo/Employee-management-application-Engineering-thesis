using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;
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
    
    //Methods for getting task
    public async Task<Response<TaskDTO>> GetTaskById(int id)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.ID == id && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .FirstOrDefaultAsync();

        return task == null
            ? Response<TaskDTO>.Fail("Task not found")
            : Response<TaskDTO>.Ok(task);
    }

    public async Task<Response<TaskWithRelatedDTO>> GetTaskWithRelatedTasks(int id)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.ID == id)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .FirstOrDefaultAsync();

        if (task == null)
            return Response<TaskWithRelatedDTO>.Fail("Task not found");

        var relatedIds = await _context.RelatedTasks
            .AsNoTracking()
            .Where(rt => rt.Main_Task_ID == id || rt.Related_Task_ID == id)
            .Select(rt => rt.Main_Task_ID == id ? rt.Related_Task_ID : rt.Main_Task_ID)
            .Distinct()
            .ToListAsync();

        var relatedDTOs = await _context.Tasks
            .AsNoTracking()
            // trzeba podjac decyzje czy filtrujemy soft-delete (&& t.Deleted == 0)
            .Where(t => relatedIds.Contains(t.ID) && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .ToListAsync();

        return Response<TaskWithRelatedDTO>.Ok(new TaskWithRelatedDTO{
            Task = task,
            RelatedTasks = relatedDTOs
        });
    }

    public async Task<Response<ICollection<TaskDTO>>> GetUserTasks(int userId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Users.Any(u => u.ID == userId) && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1,
            })
            .ToListAsync();

        return tasks.Count == 0
            ? Response<ICollection<TaskDTO>>.Fail("User has no tasks")
            : Response<ICollection<TaskDTO>>.Ok(tasks);
    }

    public async Task<Response<ICollection<TaskDTO>>> GetGroupTasks(int groupId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Groups.Any(g => g.ID == groupId) && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .ToListAsync();
        
        return tasks.Count == 0
            ? Response<ICollection<TaskDTO>>.Fail("Group has no tasks")
            : Response<ICollection<TaskDTO>>.Ok(tasks);
    }

    public async Task<Response<ICollection<TaskDTO>>> GetTasksByPriorityId(int priorityId)
    {
        var priorityExists = await _context.Priorities
            .AsNoTracking()
            .AnyAsync(p => p.ID == priorityId);
        if (!priorityExists)
            return Response<ICollection<TaskDTO>>.Fail("Priority not found");
        
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Priority_ID == priorityId && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .ToListAsync();
        return tasks.Count == 0
            ? Response<ICollection<TaskDTO>>.Fail("Priority has no tasks")
            : Response<ICollection<TaskDTO>>.Ok(tasks);
    }
    
    public async Task<Response<ICollection<TaskDTO>>> GetTasksByStatusId(int statusId)
    {
        var statusExists = await _context.Statuses
            .AsNoTracking()
            .AnyAsync(s => s.ID == statusId);
        if (!statusExists)
            return Response<ICollection<TaskDTO>>.Fail("Status not found");
        
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Status_ID == statusId && t.Deleted == 0)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Description = t.Description,
                Start_Time = t.Start_Time,
                Estimated_Time = t.Estimated_Time,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1
            })
            .ToListAsync();
        
        return tasks.Count == 0
            ? Response<ICollection<TaskDTO>>.Fail("Status has no tasks")
            : Response<ICollection<TaskDTO>>.Ok(tasks);
    }
    
    public async Task<Response<ICollection<GanttTaskDTO>>> GetGanttTasks(int userId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Users)
            .Where(t => t.Users.Any(u => u.ID == userId) && t.Deleted == 0)
            .ToListAsync();

        if (tasks.Count == 0)
            return Response<ICollection<GanttTaskDTO>>.Fail("User has no tasks");

        var taskIds = tasks.Select(t => t.ID).ToList();
        
        var relations = await _context.RelatedTasks
            .AsNoTracking()
            .Where(rt => taskIds.Contains(rt.Main_Task_ID))
            .ToListAsync();

        var dependenciesLookup = relations
            .GroupBy(rt => rt.Main_Task_ID)
            .ToDictionary(
                g => g.Key,
                g => g.Select(rt => rt.Related_Task_ID).Distinct().ToList()
            );

        var ganttTasks = tasks.Select(t =>
        {
            var endTime = t.Start_Time.AddHours(t.Estimated_Time);


            dependenciesLookup.TryGetValue(t.ID, out var deps);
            deps ??= new List<int>();

            return new GanttTaskDTO
            {
                ID = t.ID,
                Name = t.Name,
                Start_Time = t.Start_Time,
                End_Time = endTime,
                Priority = t.Priority.Priority1,
                Status = t.Status.Status1,
                OwnerUserId = userId,
                Dependencies = deps
            };
        }).ToList();

        return Response<ICollection<GanttTaskDTO>>.Ok(ganttTasks);
    }



    //Methods for creating task
    public async Task<Response<TaskDTO>> CreateTask(CreateTaskModel model)
    {
        try
        {
            var task = new Task()
            {
                Name = model.Name,
                Description = model.Description,
                Start_Time = model.Start_Time,
                Estimated_Time = model.Estimated_Time
            };

            var priority = await _context.Set<Priority>().FirstOrDefaultAsync(e => e.Priority1 == model.Priority);

            if (priority == null)
            {
                return Response<TaskDTO>.Fail("Priority not found");
            }

            var status = await _context.Set<Status>().FirstOrDefaultAsync(e => e.Status1 == model.Status);
            if (status == null)
            {
                return Response<TaskDTO>.Fail("Status not found");
            }

            task.Priority = priority;
            task.Status = status;

            await _context.Set<Task>().AddAsync(task);
            await _context.SaveChangesAsync();

            var taskDto = new TaskDTO
            {
                ID = task.ID,
                Name = task.Name,
                Description = task.Description,
                Start_Time = task.Start_Time,
                Estimated_Time = task.Estimated_Time,
                Priority = task.Priority.Priority1,
                Status = task.Status.Status1
            };

            return Response<TaskDTO>.Ok(taskDto);
        }
        catch (Exception e)
        {
            return Response<TaskDTO>.Fail($"Error during creating task: {e.Message}");
        }
    }

    public async Task<Response<TaskDTO>> CreateTaskAssignToUser(CreateTaskModel model, int userId)
    {
        var user = _context.Set<User>().FirstOrDefault(e => e.ID == userId);
        if (user == null)
        {
            return Response<TaskDTO>.Fail("User not found");
        }

        var task = await CreateTask(model);
        if (!task.Success)
        {
            return Response<TaskDTO>.Fail(task.Error);
        }

        var taskEntity = await _context.Set<Task>().FindAsync(task.Data.ID);
        taskEntity.Users.Add(user);
        await _context.SaveChangesAsync();

        var taskDto = new TaskDTO
        {
            ID = taskEntity.ID,
            Name = taskEntity.Name,
            Description = taskEntity.Description,
            Start_Time = taskEntity.Start_Time,
            Estimated_Time = taskEntity.Estimated_Time,
            Priority = taskEntity.Priority.Priority1,
            Status = taskEntity.Status.Status1
        };

        return Response<TaskDTO>.Ok(taskDto);
    }

    public async Task<Response<TaskDTO>> CreateTaskAssignToGroup(CreateTaskModel model, int groupId)
    {
        var group = _context.Set<Group>().FirstOrDefault(e => e.ID == groupId);
        if (group == null)
        {
            return Response<TaskDTO>.Fail("Group not found");
        }

        var task = await CreateTask(model);
        if (!task.Success)
        {
            return Response<TaskDTO>.Fail(task.Error);
        }

        var taskEntity = await _context.Set<Task>().FindAsync(task.Data.ID);
        taskEntity.Groups.Add(group);
        await _context.SaveChangesAsync();

        var taskDto = new TaskDTO
        {
            ID = taskEntity.ID,
            Name = taskEntity.Name,
            Description = taskEntity.Description,
            Start_Time = taskEntity.Start_Time,
            Estimated_Time = taskEntity.Estimated_Time,
            Priority = taskEntity.Priority.Priority1,
            Status = taskEntity.Status.Status1
        };

        return Response<TaskDTO>.Ok(taskDto);
    }

    //Methods for updating task
    public async Task<Response<TaskDTO>> UpdateTask(UpdateTaskDTO dto, int id)
    {
        try
        {
            var task = await _context.Set<Task>()
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(e => e.ID == id);
            if (task == null)
            {
                return Response<TaskDTO>.Fail("Task not found");
            }
            
            var priority = await _context.Set<Priority>().FirstOrDefaultAsync(e => e.Priority1 == dto.Priority);
            if (priority == null)
            {
                return Response<TaskDTO>.Fail("Priority not found");
            }

            var status = await _context.Set<Status>().FirstOrDefaultAsync(e => e.Status1 == dto.Status);
            if (status == null)
            {
                return Response<TaskDTO>.Fail("Status not found");
            }

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Start_Time = dto.Start_Time;
            task.Estimated_Time = dto.Estimated_Time;
            task.Priority = priority;
            task.Status = status;
            await _context.SaveChangesAsync();
            
            var taskDto = new TaskDTO
            {
                ID = task.ID, 
                Name = task.Name, 
                Description = task.Description, 
                Start_Time = task.Start_Time, 
                Estimated_Time = task.Estimated_Time, 
                Priority = task.Priority.Priority1, 
                Status = task.Status.Status1
            };
            
            return Response<TaskDTO>.Ok(taskDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<TaskDTO>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<TaskDTO>.Fail($"Error during updating task: {e.Message}");
        }
    }
    
    //Methods for deleting task
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

    //Methods for managing relations
    public async Task<Response<TaskRelationDTO>> AddRelation(int taskId, int relatedTaskId)
    {
        try
        {
            if (taskId == relatedTaskId)
                return Response<TaskRelationDTO>.Fail("Cannot relate task to itself");

            var ids = await _context.Tasks
                .AsNoTracking()
                .Where(t => (t.ID == taskId || t.ID == relatedTaskId) && t.Deleted == 0)
                .Select(t => t.ID)
                .ToListAsync();

            if (ids.Count != 2)
                return Response<TaskRelationDTO>.Fail("Task not found");

            var exists = await _context.RelatedTasks
                .AsNoTracking()
                .AnyAsync(rt =>
                    (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                    (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId));
            if (exists)
                return Response<TaskRelationDTO>.Fail("Relation already exists");

            var relation = new RelatedTask
            {
                Main_Task_ID = taskId,
                Related_Task_ID = relatedTaskId,
            };
            _context.RelatedTasks.Add(relation);
            await _context.SaveChangesAsync();

            return Response<TaskRelationDTO>.Ok(new TaskRelationDTO(relation.Main_Task_ID, relation.Related_Task_ID));
        }
        catch (DbUpdateException e)
        {
            return Response<TaskRelationDTO>.Fail("Relation already exists");
        }
        catch (Exception e)
        {
            return Response<TaskRelationDTO>.Fail($"Error during creating relation: {e.Message}");
        }
    }
    
    public async Task<Response<object>> RemoveRelation(int taskId, int relatedTaskId)
    {
        try
        {
            var relation = await _context.RelatedTasks
                .FirstOrDefaultAsync(rt =>
                    (rt.Main_Task_ID == taskId && rt.Related_Task_ID == relatedTaskId) ||
                    (rt.Main_Task_ID == relatedTaskId && rt.Related_Task_ID == taskId));
            if (relation == null)
                return Response<object>.Fail("Relation not found");
            
            _context.RelatedTasks.Remove(relation);
            await _context.SaveChangesAsync();
            return Response<object>.Ok(new { removed = true });
        }
        catch (Exception e)
        {
            return Response<object>.Fail($"Error during removing relation: {e.Message}");
        }
    }
}