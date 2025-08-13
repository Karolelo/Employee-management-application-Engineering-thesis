using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.task;
using Repo.Core.Models.DTOs;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.TaskModule.interafaces;

public interface ITaskService
{
    //Methods for getting task
    Task<Response<ICollection<Repo.Core.Models.Task>>> GetUserTasks(int userId);
    Task<Response<Repo.Core.Models.Task>> GetTaskById(int id);
    
    Task<Response<TaskWithRelatedDTO>> GetTaskWithRelatedTasks(int id);
    Task<Response<ICollection<Repo.Core.Models.Task>>> GetGroupTasks(int groudId);

    //Methods for creating task
    Task<Response<Repo.Core.Models.Task>> CreateTask(CreateTaskModel model);
    Task<Response<Repo.Core.Models.Task>> CreateTaskAssignToUser(CreateTaskModel model, int userId);
    Task<Response<Repo.Core.Models.Task>> CreateTaskAssignToGroup(CreateTaskModel model, int groupId);

    //Methods for updating task
    Task<Response<Repo.Core.Models.DTOs.TaskDTO>> UpdateTask(UpdateTaskModel model, int id);
    Task<Response<Repo.Core.Models.Task>> UpdateTaskStatus(int id, int status);
    
    //Methods for deleting task
    Task<Response<Task>> DeleteTask(int id);
}