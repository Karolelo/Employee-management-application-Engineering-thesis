using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Infrastructure.Files;
using Repo.Core.Infrastructure.Roles;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IFileOperations _file;
    public GroupService(IGroupRepository groupRepository
        , IFileOperations file
        , IOptions<RoleConfiguration> roleConfiguration
        )
    {
        _groupRepository = groupRepository;
        _file = file;
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
                Description = dto.Description,
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
            existingGroup.Description = dto.Description;

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

    public async Task<Response<string>> GetGroupImagePath(int groupId)
    {
        try
        {
            var path = await _groupRepository.GetPathToImageFile(groupId);
            
            return path.IsNullOrEmpty() ? Response<string>.Fail("Group does not have image") : Response<string>.Ok(path);
        }
        catch (Exception ex)
        {
            return Response<string>.Fail($"Error while fetching image path: {ex.Message}");
        }
    }
    
    public async Task<Response<string>> SaveGroupImage(int groupId, IFormFile image,bool isUpdate = false)
    {
        try
        {
            var group = await _groupRepository.GetGroupById(groupId);
            if (group == null)
            {
                return Response<string>.Fail("Group with this id not found");
            }
            
            var fileName = $"group_{groupId}_{DateTime.Now.Ticks}{Path.GetExtension(image.FileName)}";
            var relativePath = Path.Combine("images", "groups", fileName);
            var absolutePath = Path.Combine("wwwroot", relativePath);
            
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

            if (isUpdate)
            {
                var path = await _groupRepository.GetPathToImageFile(groupId);
                var pathToDelete = Path.Combine("wwwroot", path);
                if (File.Exists(pathToDelete))
                    File.Delete(pathToDelete);
                await _groupRepository.UpdateImageFile(groupId, relativePath);
            }
            else
            {
                await _groupRepository.SavePathToImageFile(groupId, relativePath);
            }

            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms); ;
                var fileBytes = ms.ToArray();
                _file.SaveFile(absolutePath, fileBytes);
            }
            
            return Response<string>.Ok(relativePath);
        }
        catch (Exception ex)
        {
            return Response<string>.Fail($"Error while saving image: {ex.Message}");
        }
    }

    private GroupDTO MapToGroupDTO(Group group)
    {
        return new GroupDTO()
        {
            ID = group.ID,
            Name = group.Name,
            Admin_ID = group.Admin_ID,
            Description = group.Description,
        };
    }
}