using System.Collections.Immutable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;
using Repo.Core.Models.task;
using Repo.Server.TaskModule.interafaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule;
public class TaskService(ITaskRepository taskRepository,IUserService userService,MyDbContext context) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;

    private readonly IUserService _userService = userService;
    //Unfortunately we need this is some places, if we create repository for status
    //and priority, we going to change this
    private readonly MyDbContext _context = context;
    //Methods for getting task
    public async Task<Response<TaskDTO>> GetTaskById(int id)
    {
        var task = await _taskRepository.GetTaskById(id);

        return task == null
            ? Response<TaskDTO>.Fail("Task not found")
            : Response<TaskDTO>.Ok(MapTaskToDto(task));
    }

    public async Task<Response<TaskWithRelatedDTO>> GetTaskWithRelatedTasks(int id)
    {
        var task = await _taskRepository.GetTaskById(id);

        if (task == null)
            return Response<TaskWithRelatedDTO>.Fail("Task not found");

        var relatedIds = await _taskRepository.GetRelatedTaskIds(id);

        var relatedTasks = await GetTaskByIds(relatedIds);

        return Response<TaskWithRelatedDTO>.Ok(new TaskWithRelatedDTO{
            Task = MapTaskToDto(task),
            RelatedTasks = relatedTasks.Select(MapTaskToDto).ToImmutableList()
        });
    }

    public async Task<Response<ICollection<TaskDTO>>> GetUserTasks(int userId)
    {
        try
        {
            var tasks = await _taskRepository.GetUserTasks(userId);
            return Response<ICollection<TaskDTO>>.Ok(tasks.Select(MapTaskToDto).ToList());
        }
        catch (Exception e)
        {
            return Response<ICollection<TaskDTO>>.Fail($"Exception during getting tasks from user: {e.Message}");
        }
    }

    public async Task<Response<ICollection<TaskDTO>>> GetGroupTasks(int groupId)
    {
        try
        {
            var tasks = await _taskRepository.GetGroupTasks(groupId);
            return Response<ICollection<TaskDTO>>.Ok(tasks.Select(MapTaskToDto).ToList());
        }
        catch (Exception e)
        {
            return Response<ICollection<TaskDTO>>.Fail($"Exception during getting tasks from user: {e.Message}");
        }
    }
    //Methods for creating task
    public async Task<Response<TaskDTO>> CreateTask(CreateTaskModel model)
    {
        try
        {
            var task = await MapCreateTaskModelToTask(model);

            await _context.Set<Task>().AddAsync(task);
            await _context.SaveChangesAsync();

            var taskDto = MapTaskToDto(task);

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
            var task = await _taskRepository.GetTaskById(id);
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
            var task = await _taskRepository.GetTaskById(id);

            if (task == null)
            {
                return Response<Task>.Fail("Task not found");
            }

            await _taskRepository.DeleteTask(id);
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

            var ids = await GetTaskByIds(new List<int>{taskId, relatedTaskId});

            if (ids.Count != 2)
                return Response<TaskRelationDTO>.Fail("Task not found");

            var exists = await _taskRepository.RelationExists(taskId, relatedTaskId);
            if (exists)
                return Response<TaskRelationDTO>.Fail("Relation already exists");

            var relation = new RelatedTask
            {
                Main_Task_ID = taskId,
                Related_Task_ID = relatedTaskId,
            };
            await _taskRepository.AddRelation(taskId, relatedTaskId);

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
            if(!await _taskRepository.RelationExists(taskId,relatedTaskId))
                return Response<object>.Fail("Relation not found");

            _taskRepository.RemoveRelation(taskId, relatedTaskId);
            return Response<object>.Ok(new { removed = true });
        }
        catch (Exception e)
        {
            return Response<object>.Fail($"Error during removing relation: {e.Message}");
        }
    }
    
    //Helpers
    private TaskDTO MapTaskToDto(Task task)
    {
        return new TaskDTO()
        {
            ID = task.ID,
            Name = task.Name,
            Description = task.Description,
            Start_Time = task.Start_Time,
            Estimated_Time = task.Estimated_Time,
            Priority = task.Priority.Priority1,
            Status = task.Status.Status1
        };
    }

    private async Task<Task> MapCreateTaskModelToTask(CreateTaskModel task)
    {
        var taskInformation = await ValidatePriorityAndStatus(task.Priority, task.Status);
        return new Task()
        {
            Name = task.Name,
            Description = task.Description,
            Start_Time = task.Start_Time,
            Estimated_Time = task.Estimated_Time,
            Priority = taskInformation.Item1,
            Status = taskInformation.Item2,
            Creator_ID = task.Creator_ID
        };
    }

    private async Task<ICollection<Task>> GetTaskByIds(List<int> ids)
    {
        var tasks = new List<Task>();
        foreach (var id in ids)
        {
            var task = await _taskRepository.GetTaskById(id);
            if (task != null)
            {
                tasks.Add(task);
            }
        }
        return tasks;
    }
    //Do not validate anything because services got validation inside
    public async Task<Tuple<Priority, Status>> ValidatePriorityAndStatus(string? priority,string? status)
    {
        var priorityResult =  await _context.Priorities
            .FirstOrDefaultAsync(p => p.Priority1 == priority);

        if (priorityResult == null)
            throw new ArgumentException("Priority with this name doesn't exists");
        
        var statusResult = await _context.Statuses
            .FirstOrDefaultAsync(s => s.Status1 == status);

        if (statusResult == null)
            throw new ArgumentException("Status with this name doesn't exists");
        return new Tuple<Priority, Status>(priorityResult, statusResult);
    }
}