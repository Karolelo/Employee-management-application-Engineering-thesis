using Repo.Core.Models.api;
using Repo.Core.Models.user;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IGroupService
{
    Task<Response<List<GroupDTO>>> GetAllGroups();
    Task<Response<GroupDTO>> GetGroupById(int id);
    Task<Response<List<GroupDTO>>> GetUsersGroups(int userId);
    Task<Response<GroupDTO>> GetGroupByAdminId(int adminId);
    Task<Response<GroupDTO>> CreateGroup(CreateGroupDTO dto);
    Task<Response<GroupDTO>> UpdateGroup(UpdateGroupDTO group);
    Task<Response<bool>> DeleteGroup(int id);
    Task<Response<bool>> AddUserToGroup(int userId, int groupId);
    Task<Response<bool>> RemoveUserFromGroup(int userId, int groupId);
    Task<Response<bool>> SetLeaderOfGroup(int userId, int groupId);
    Task<Response<string>> GetGroupImagePath(int groupId);
    Task<Response<string>> SaveGroupImage(int groupId, IFormFile image, bool isUpdate = false);
    
}