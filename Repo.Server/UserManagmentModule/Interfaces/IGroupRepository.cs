using Repo.Core.Models;
using Repo.Core.Models.user;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IGroupRepository
{
    Task<List<Group>> GetAllGroups();
    Task<Group?> GetGroupById(int id);
    Task<Group> CreateGroup(Group group);
    Task<Group> UpdateGroup(Group group);
    Task<bool> DeleteGroup(int id);
    Task<bool> AddUserToGroup(int userId, int groupId);
    Task<bool> RemoveUserFromGroup(int userId, int groupId);
    Task<bool> AddTaskToGroup(int groupId, int taskId);
    Task<bool> SetLeaderOfGroup(int userId, int groupId);
    Task<string> GetPathToImageFile(int groupId);
    Task<string> SavePathToImageFile(int groupId, string path);
    Task<string> UpdateImageFile(int groupId, string path);
}