using Microsoft.Extensions.Options;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.Roles;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = Repo.Core.Models.Task;

namespace Repo.Server.UserManagmentModule.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly RoleConfiguration _roleConfiguration;
    public UserService(IUserRepository userRepository
        ,IRoleRepository roleRepository
        ,IGroupRepository groupRepository
        ,IOptions<RoleConfiguration> roleConfiguration)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _groupRepository = groupRepository;
        _roleConfiguration = roleConfiguration.Value;
    }

    public async Task<Response<List<UserDTO>>> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllUsers();
            var usersDto = users.Select(user => new UserDTO(user)).ToList();
            return Response<List<UserDTO>>.Ok(usersDto);
        }
        catch (Exception ex)
        {
            return Response<List<UserDTO>>.Fail($"Error while fetching users: {ex.Message}");
        }
    }

    public async Task<Response<List<UserDTO>>> GetAllUsersFromGroup(int groupId)
    {
        try
        {
            if (await _groupRepository.GetGroupById(groupId) == null)
            {
                return Response<List<UserDTO>>.Fail("Group with this id not found");
            }
            var users = await _userRepository.GetAllUsersFromGroup(groupId);
            var usersDto = users.Select(user => new UserDTO(user)).ToList();
            return Response<List<UserDTO>>.Ok(usersDto);
        }
        catch (Exception e)
        {
            return Response<List<UserDTO>>.Fail($"Error while fetching users: {e.Message}");
        }
    }

    public async Task<Response<List<UserDTO>>> GetUsersWithoutGroup()
    {
        try
        {
            var users = await _userRepository.GetAllUsers();
            var usersWithoutGroup = users.Where(u => !u.Groups.Any()).ToList();
            var usersDto = usersWithoutGroup.Select(user => new UserDTO(user)).ToList();
            return Response<List<UserDTO>>.Ok(usersDto);
        }
        catch (Exception e)
        {
            return Response<List<UserDTO>>.Fail($"Error while fetching users: {e.Message}");
        }
    }

    public async Task<Response<List<UserDTO>>> GetTeamLeadersWithoutGroup()
    {
        try
        {
            var response = await GetAllUsers();

            if (response.Success)
            {
                var role = "TeamLeader";
                if (!_roleConfiguration.AvailableRoles.Contains(role))
                    return Response<List<UserDTO>>.Fail($"Role: {role} is not available");
                
                var users = response.Data
                    .Where(u => u.Roles.Contains(role)&&!_userRepository.IsAdminHasGroup(u.ID))
                    .ToList();
                
                return Response<List<UserDTO>>.Ok(users);
            }
            return Response<List<UserDTO>>.Fail("No available teamLead found");
        }
        catch (Exception e)
        {
            return Response<List<UserDTO>>.Fail($"Error while fetching users: {e.Message}");
        }
    }
    
    public async Task<Response<UserDTO>> GetUserById(int id)
    {
        try
        {
            var user = await _userRepository.GetUserById(id);
            return user != null 
                ? Response<UserDTO>.Ok(new UserDTO(user)) 
                : Response<UserDTO>.Fail($"User with ID: {id} not found");
        }
        catch (Exception ex)
        {
            return Response<UserDTO>.Fail($"Error while fetching user: {ex.Message}");
        }
    }

    public async Task<Response<List<UserDTO>>> GetUsersWithRole(string role)
    {
        try
        {
            var response = await GetAllUsers();

            if (response.Success)
            {
                if(!_roleConfiguration.AvailableRoles.Contains(role))
                    return Response<List<UserDTO>>.Fail($"Role: {role} is not available");
                
                var users = response.Data.Where(u=>u.Roles.Contains(role)).ToList();
                return Response<List<UserDTO>>.Ok(users);
            }
            return Response<List<UserDTO>>.Fail($"Error during fetching users with role: {role}, Error: {response.Error}");

        }
        catch (Exception ex)
        {
            return Response<List<UserDTO>>.Fail($"Error while fetching users: {ex.Message}");
        }
    }

    public async Task<Response<UserDTO>> UpdateUser(UserUpdateDTO dto)
    {
        try
        {
            if (dto == null)
                return Response<UserDTO>.Fail("Update data cannot be null");
            
            var existingUser = await _userRepository.GetUserById(dto.ID);
            if (existingUser == null)
                return Response<UserDTO>.Fail($"User with ID: {dto.ID} not found");

            
            var userWithEmail = await _userRepository.GetUserByEmail(dto.Email.Trim());
            if (userWithEmail != null && userWithEmail.ID != dto.ID)
                return Response<UserDTO>.Fail("Email is already taken");

            
            if (!string.IsNullOrEmpty(dto.Nickname))
            {
                var userWithNickname = await _userRepository.GetUserByNickname(dto.Nickname.Trim());
                if (userWithNickname != null && userWithNickname.ID != dto.ID)
                    return Response<UserDTO>.Fail("Nickname is already taken");
            }

            var resultRoles = await _roleRepository.GetAllRoles();
            var userRoles = resultRoles.Where(r => dto.Roles.Contains(r.Role_Name)).ToList();
            //We doing this becuase of update method
            //Using dtos made me to do that, but long term
            //I think there are way to skip this, but
            //for now it works quiet good
            var updatedUser = new User
            {
                ID = existingUser.ID,
                Name = dto.Name.Trim(),
                Surname = dto.Surname?.Trim(),
                Email = dto.Email.Trim(),
                Login = dto.Login?.Trim(),
                Nickname = dto.Nickname?.Trim(),
                Salt = existingUser.Salt,
                Roles = userRoles
            };

            
            if (!string.IsNullOrEmpty(dto.Password))
            {
                updatedUser.Password = AuthenticationHelpers.GeneratePasswordHash(dto.Password, existingUser.Salt);
            }
            else
            {
                updatedUser.Password = existingUser.Password; //Leaving old password 
            }
            
            var updated = await _userRepository.UpdateUser(updatedUser);
            
            return Response<UserDTO>.Ok(new UserDTO(updated));
        }
        catch (Exception ex)
        {
            return Response<UserDTO>.Fail($"Error while updating user: {ex.Message}");
        }
    }

    public async Task<Response<bool>> DeleteUser(int id)
    {
        try
        {
            var result = await _userRepository.DeleteUser(id);
            return result 
                ? Response<bool>.Ok(true) 
                : Response<bool>.Fail($"Failed to delete user with ID: {id}");
        }
        catch (Exception ex)
        {
            return Response<bool>.Fail($"Error while deleting user: {ex.Message}");
        }
    }
    
}