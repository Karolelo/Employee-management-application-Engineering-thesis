using Repo.Core.Models;
using Repo.Core.Models.api;

namespace Repo.Server.TaskModule.interafaces;

public interface ITaskManager
{
    //Methods for getting task
    Task<ICollection<Repo.Core.Models.Task>> GetUserTasks(User user);
    Task<Repo.Core.Models.Task> GetTaskById(int id);
    Task<Response<ICollection<Repo.Core.Models.Task>>> GetGroupTask(int status);
    
    //Methods for creating task
    Task<Repo.Core.Models.Task> CreateTask(Repo.Core.Models.Task task);
    Task<Repo.Core.Models.Task> CreateTaskAssignToUser(Repo.Core.Models.Task task,int userId);
    Task<Repo.Core.Models.Task> CreateTaskAssignToGroup(Repo.Core.Models.Task task,int groupId);
    
    //Methods for updating task
    Task<Repo.Core.Models.Task> UpdateTask(Repo.Core.Models.Task task);
    Task<Repo.Core.Models.Task> UpdateTaskStatus(int id,int status);
    
    //Methods for deleting task
    void DeleteTask(int id);
}