using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    
    public UserService(IUserRepository userRepository,IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<Response<List<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllUsers();
            var usersDto = users.Select(user => new UserDto(user)).ToList();
            return Response<List<UserDto>>.Ok(usersDto);
        }
        catch (Exception ex)
        {
            return Response<List<UserDto>>.Fail($"Error while fetching users: {ex.Message}");
        }
    }

    public async Task<Response<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _userRepository.GetUserById(id);
            return user != null 
                ? Response<UserDto>.Ok(new UserDto(user)) 
                : Response<UserDto>.Fail($"User with ID: {id} not found");
        }
        catch (Exception ex)
        {
            return Response<UserDto>.Fail($"Error while fetching user: {ex.Message}");
        }
    }
    public async Task<Response<UserDto>> UpdateUser(UserUpdateDTO dto)
    {
        try
        {
            if (dto == null)
                return Response<UserDto>.Fail("Update data cannot be null");
            
            var existingUser = await _userRepository.GetUserById(dto.ID);
            if (existingUser == null)
                return Response<UserDto>.Fail($"User with ID: {dto.ID} not found");

            
            var userWithEmail = await _userRepository.GetUserByEmail(dto.Email.Trim());
            if (userWithEmail != null && userWithEmail.ID != dto.ID)
                return Response<UserDto>.Fail("Email is already taken");

            
            if (!string.IsNullOrEmpty(dto.Nickname))
            {
                var userWithNickname = await _userRepository.GetUserByNickname(dto.Nickname.Trim());
                if (userWithNickname != null && userWithNickname.ID != dto.ID)
                    return Response<UserDto>.Fail("Nickname is already taken");
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
                updatedUser.Password = existingUser.Password; //Zachowuje stare hasło 
            }
            
            var updated = await _userRepository.UpdateUser(updatedUser);
            
            return Response<UserDto>.Ok(new UserDto(updated));
        }
        catch (Exception ex)
        {
            return Response<UserDto>.Fail($"Error while updating user: {ex.Message}");
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