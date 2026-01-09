using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.task;
using Repo.Core.Models.DTOs;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule.interafaces;

public interface ITaskService
{
    //Methods for getting task
    Task<Response<TaskDTO>> GetTaskById(int id);
    Task<Response<TaskWithRelatedDTO>> GetTaskWithRelatedTasks(int id);
    Task<Response<ICollection<TaskDTO>>> GetUserTasks(int userId);
    Task<Response<ICollection<TaskDTO>>> GetGroupTasks(int groupId);

    //Methods for creating task
    Task<Response<TaskDTO>> CreateTask(CreateTaskModel model);
    Task<Response<TaskDTO>> CreateTaskAssignToUser(CreateTaskModel model, int userId);
    Task<Response<TaskDTO>> CreateTaskAssignToGroup(CreateTaskModel model, int groupId);

    //Methods for updating task
    Task<Response<TaskDTO>> UpdateTask(UpdateTaskDTO dto, int id);
    
    //Methods for deleting task
    Task<Response<Task>> DeleteTask(int id);
    
    //Methods for managing relations
    Task<Response<TaskRelationDTO>> AddRelation(int taskId, int relatedTaskId);
    Task<Response<object>> RemoveRelation(int taskId, int relatedTaskId);
}