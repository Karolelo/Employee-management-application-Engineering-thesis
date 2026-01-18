using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.DTOs;
using Repo.Core.Models.task;
using Repo.Core.Models.user;
using Repo.Server.TaskModule.interafaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule;
public class TaskService(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IGroupRepository groupRepository,
    IPriorityRepository priorityRepository,
    IStatusRepository statusRepository) : ITaskService
{
    //Methods for getting task
    public async Task<Response<TaskDTO>> GetTaskById(int id)
    {
        var task = await taskRepository.GetTaskById(id);

        return task == null
            ? Response<TaskDTO>.Fail("Task not found")
            : Response<TaskDTO>.Ok(MapTaskToDto(task));
    }

    public async Task<Response<TaskWithRelatedDTO>> GetTaskWithRelatedTasks(int id)
    {
        var task = await taskRepository.GetTaskById(id);

        if (task == null)
            return Response<TaskWithRelatedDTO>.Fail("Task not found");

        var relatedIds = await taskRepository.GetRelatedTaskIds(id);

        var relatedTasks = await GetTaskByIds(relatedIds);

        return Response<TaskWithRelatedDTO>.Ok(new TaskWithRelatedDTO
        {
            Task = MapTaskToDto(task),
            RelatedTasks = relatedTasks.Select(MapTaskToDto).ToImmutableList()
        });
    }

    public async Task<Response<ICollection<TaskDTO>>> GetUserTasks(int userId)
    {
        try
        {
            var tasks = await taskRepository.GetUserTasks(userId);
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
            var tasks = await taskRepository.GetGroupTasks(groupId);
            return Response<ICollection<TaskDTO>>.Ok(tasks.Select(MapTaskToDto).ToList());
        }
        catch (Exception e)
        {
            return Response<ICollection<TaskDTO>>.Fail($"Exception during getting tasks from group: {e.Message}");
        }
    }

    public async Task<Response<ICollection<GanttTaskDTO>>> GetGanttTasks(int userId)
    {
        var tasks = await taskRepository.GetUserTasksForGantt(userId);

        if (!tasks.Any())
            return Response<ICollection<GanttTaskDTO>>.Fail("User has no tasks");

        var taskIds = tasks.Select(t => t.ID).ToList();
        var relations = await taskRepository.GetRelatedTasksByMainTaskIds(taskIds);

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
            var validationResult = await ValidatePriorityAndStatus(model.Priority, model.Status);
            if (!validationResult.Success)
                return Response<TaskDTO>.Fail(validationResult.Error);

            var (priority, status) = validationResult.Data;

            var task = new Task
            {
                Name = model.Name,
                Description = model.Description,
                Start_Time = model.Start_Time,
                Estimated_Time = model.Estimated_Time,
                Priority_ID = priority.ID,
                Status_ID = status.ID,
                Creator_ID = model.Creator_ID
            };

            var createdTask = await taskRepository.CreateTask(task);
            var taskDto = MapTaskToDto(createdTask);

            return Response<TaskDTO>.Ok(taskDto);
        }
        catch (Exception e)
        {
            return Response<TaskDTO>.Fail($"Error during creating task: {e.Message}");
        }
    }

    public async Task<Response<TaskDTO>> CreateTaskAssignToUser(CreateTaskModel model, int userId)
    {
        if (!await userRepository.UserExists(userId))
        {
            return Response<TaskDTO>.Fail("User not found");
        }

        var task = await CreateTask(model);
        if (!task.Success)
        {
            return Response<TaskDTO>.Fail(task.Error);
        }

        int taskId = task.Data.ID;
        await userRepository.AddTaskToUser(userId, taskId);

        return Response<TaskDTO>.Ok(task.Data);
    }

    public async Task<Response<TaskDTO>> CreateTaskAssignToGroup(CreateTaskModel model, int groupId)
    {
        try
        {
            if (await groupRepository.GetGroupById(groupId) == null)
            {
                return Response<TaskDTO>.Fail("Group with found");
            }
            
            var task = await CreateTask(model);
            var taskId = task.Data.ID;
            await groupRepository.AddTaskToGroup(groupId, taskId);
            
            return Response<TaskDTO>.Ok(task.Data);
        }
        catch (Exception e)
        {
            return Response<TaskDTO>.Fail($"Error during assigning task to group: {e.Message}");
        }
    }

    //Methods for updating task
    public async Task<Response<TaskDTO>> UpdateTask(UpdateTaskDTO dto, int id)
    {
        try
        {
            var task = await taskRepository.GetTaskById(id);
            if (task == null)
            {
                return Response<TaskDTO>.Fail("Task not found");
            }

            var validationResult = await ValidatePriorityAndStatus(dto.Priority, dto.Status);
            if (!validationResult.Success)
                return Response<TaskDTO>.Fail(validationResult.Error);

            var (priority, status) = validationResult.Data;

            task.Name = dto.Name;
            task.Description = dto.Description;
            task.Start_Time = dto.Start_Time;
            task.Estimated_Time = dto.Estimated_Time;
            task.Priority_ID = priority.ID;
            task.Status_ID = status.ID;

            await taskRepository.UpdateTask(task);

            var taskDto = MapTaskToDto(task);

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
            var task = await taskRepository.GetTaskById(id);

            if (task == null)
            {
                return Response<Task>.Fail("Task not found");
            }

            await taskRepository.DeleteTask(id);
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

            var ids = await GetTaskByIds(new List<int> { taskId, relatedTaskId });

            if (ids.Count != 2)
                return Response<TaskRelationDTO>.Fail("Task not found");

            var exists = await taskRepository.RelationExists(taskId, relatedTaskId);
            if (exists)
                return Response<TaskRelationDTO>.Fail("Relation already exists");

            var relation = new RelatedTask
            {
                Main_Task_ID = taskId,
                Related_Task_ID = relatedTaskId,
            };
            await taskRepository.AddRelation(taskId, relatedTaskId);

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
            if (!await taskRepository.RelationExists(taskId, relatedTaskId))
                return Response<object>.Fail("Relation not found");

            await taskRepository.RemoveRelation(taskId, relatedTaskId);
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
        return new TaskDTO
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

    private async Task<ICollection<Task>> GetTaskByIds(List<int> ids)
    {
        var tasks = new List<Task>();
        foreach (var id in ids)
        {
            var task = await taskRepository.GetTaskById(id);
            if (task != null)
            {
                tasks.Add(task);
            }
        }
        return tasks;
    }

    /// <summary>
    /// Help method which just returning me objects of 
    /// </summary>
    public async Task<Response<(Priority, Status)>> ValidatePriorityAndStatus(string? priority, string? status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(priority) || string.IsNullOrWhiteSpace(status))
                return Response<(Priority, Status)>.Fail("Priority and Status cannot be empty");

            var priorityObj = await priorityRepository.GetPriorityByName(priority);
            if (priorityObj == null)
                return Response<(Priority, Status)>.Fail("Priority with this name not exists");

            var statusObj = await statusRepository.GetStatusByName(status);
            if (statusObj == null)
                return Response<(Priority, Status)>.Fail("Status with this name not exists");

            return Response<(Priority, Status)>.Ok((priorityObj, statusObj));
        }
        catch (Exception e)
        {
            return Response<(Priority, Status)>.Fail($"Error validating priority and status: {e.Message}");
        }
    }
}