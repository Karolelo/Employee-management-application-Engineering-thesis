using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;

    public GroupService(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Response<List<GroupDTO>>> GetAllGroups()
    {
        try
        {
            var result = await _groupRepository.GetAllGroups();

            return Response<List<GroupDTO>>.Ok(result.Select(MapToGroupDTO).ToList());
        }catch (Exception ex)
        {
            return Response<List<GroupDTO>>.Fail($"Error while fetching groups: {ex.Message}");
        }
    }

    public async Task<Response<GroupDTO?>> GetGroupById(int id)
    {
        try
        {
            var result = await _groupRepository.GetGroupById(id);

            return result is null
                ? Response<GroupDTO?>.Fail("Group with this id not founded")
                : Response<GroupDTO?>.Ok(MapToGroupDTO(result));
        }catch (Exception ex)
        {
            return Response<GroupDTO?>.Fail($"Error while fetching group: {ex.Message}");
        }
    }

    public async Task<Response<GroupDTO>> CreateGroup(CreateGroupDTO dto)
    {
        try
        {
            var group = new Group()
            {
                Name = dto.Name,
                Admin_ID = dto.Admin_ID,
                Deleted = 0
            };
            //I dont know if this mapping make big change, but 
            //I think sometimes id might be usefull especially during async operation
            //in angular
            var result = await _groupRepository.CreateGroup(group);
            return Response<GroupDTO>.Ok(MapToGroupDTO(result));
        }catch (Exception ex)
        {
            return Response<GroupDTO>.Fail($"Error while creating group: {ex.Message}");
        }
    }

    public async Task<Response<GroupDTO>> UpdateGroup(UpdateGroupDTO dto)
    {
        try
        {
            var existingGroup = await _groupRepository.GetGroupById(dto.ID);
            if (existingGroup == null)
            {
                return Response<GroupDTO>.Fail("Group does not exist");
            }
            
            existingGroup.Name = dto.Name;
            existingGroup.Admin_ID = dto.Admin_ID;

            var result = await _groupRepository.UpdateGroup(existingGroup);
            return Response<GroupDTO>.Ok(MapToGroupDTO(result));
        }
        catch (Exception ex)
        {
            return Response<GroupDTO>.Fail($"Error during updating: {ex.Message}");
        }
    }

    public async Task<Response<bool>> DeleteGroup(int id)
    {
        try
        {
            var result = await _groupRepository.DeleteGroup(id);
            return result
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail($"Failed to delete group with ID: {id}");
        }catch (Exception ex)
        {
            return Response<bool>.Fail($"Error while deleting group: {ex.Message}");
        }
    }

    public async Task<Response<bool>> AddUserToGroup(int userId, int groupId)
    {
        try
        {
            var result = await _groupRepository.AddUserToGroup(userId, groupId);
            return result
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail($"Failed to add user to group with ID: {groupId}");
        }catch (Exception ex)
        {
            return Response<bool>.Fail($"Error while adding user to group: {ex.Message}");
        }
    }

    public async Task<Response<bool>> RemoveUserFromGroup(int userId, int groupId)
    {
        try
        {
            var result = await _groupRepository.RemoveUserFromGroup(userId, groupId);
            return result
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail($"Failed to remove user from group with ID: {groupId}");
        }catch (Exception ex)
        {
            return Response<bool>.Fail($"Error while removing user from group: {ex.Message}");
        }
    }

    public async Task<Response<bool>> SetLeaderOfGroup(int userId, int groupId)
    {
        try
        {
            var result = await _groupRepository.SetLeaderOfGroup(userId, groupId);
            return result
                ? Response<bool>.Ok(true)
                : Response<bool>.Fail($"Failed to set leader of group with ID: {groupId}");
        }catch (Exception ex)
        {
            return Response<bool>.Fail($"Error while setting leader of group: {ex.Message}");
        }
    }

    private GroupDTO MapToGroupDTO(Group group)
    {
        return new GroupDTO()
        {
            ID = group.ID,
            Name = group.Name,
            Admin_ID = group.Admin_ID,
        };
    }
}