using Repo.Core.Models;
using Repo.Core.Models.DTOs;
using Task = System.Threading.Tasks.Task;

namespace Repo.Server.TaskModule.interafaces;

public interface ITaskRepository
{
    // Get operations
    Task<Core.Models.Task?> GetTaskById(int id);
    Task<IEnumerable<Core.Models.Task>> GetUserTasks(int userId);
    Task<IEnumerable<Core.Models.Task>> GetGroupTasks(int groupId);
    
    // Create operations
    Task<Core.Models.Task> CreateTask(Core.Models.Task task);
    
    // Update operations
    Task<Core.Models.Task> UpdateTask(Core.Models.Task task);
    
    // Delete operations
    Task<bool> DeleteTask(int id);
    
    // Relations
    Task<RelatedTask> AddRelation(int taskId, int relatedTaskId);
    Task<bool> RemoveRelation(int taskId, int relatedTaskId);
    Task<List<int>> GetRelatedTaskIds(int taskId);
    Task<bool> RelationExists(int taskId, int relatedTaskId);
    
    // Helpers
    Task<bool> TaskExists(int taskId);
}