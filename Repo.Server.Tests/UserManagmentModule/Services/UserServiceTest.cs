using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repo.Core.Infrastructure.Roles;
using Repo.Core.Models;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;
using Repo.Server.UserManagmentModule.Services;
namespace Repo.Server.Tests.UserManagmentModule.Services;

using Task = System.Threading.Tasks.Task;

[TestClass]
[TestSubject(typeof(UserService))]
public class UserServiceTest
{
    
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRoleRepository> _mockRoleRepository;
    private Mock<IGroupRepository> _mockGroupRepository;
    private Mock<IOptions<RoleConfiguration>> _mockRoleOptions;
    private IUserService _userService;

    [TestInitialize]
    public void SetUp()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockGroupRepository = new Mock<IGroupRepository>();
        _mockRoleOptions = new Mock<IOptions<RoleConfiguration>>();

        var roleConfig = new RoleConfiguration
        {
            AvailableRoles = ["TeamLeader", "Admin", "User"]
        };
        _mockRoleOptions.Setup(x => x.Value).Returns(roleConfig);

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockGroupRepository.Object,
            _mockRoleOptions.Object);
    }

    #region GetAllUsers Tests

    [TestMethod]
    public async Task GetAllUsers_ShouldReturnListOfUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role>() },
            new User { ID = 2, Name = "Jane", Email = "jane@test.com", Roles = new List<Role>() }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsers();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("John");
        result.Data[1].Name.Should().Be("Jane");
    }

    [TestMethod]
    public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllUsers();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetAllUsers_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.GetAllUsers();

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region GetAllUsersFromGroup Tests

    [TestMethod]
    public async Task GetAllUsersFromGroup_ShouldReturnUsersFromGroup_WhenGroupExists()
    {
        // Arrange
        int groupId = 1;
        var group = new Group { ID = groupId, Name = "TestGroup" };
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role>() }
        };
        _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync(group);
        _mockUserRepository.Setup(x => x.GetAllUsersFromGroup(groupId)).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersFromGroup(groupId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data[0].Name.Should().Be("John");
    }

    [TestMethod]
    public async Task GetAllUsersFromGroup_ShouldReturnFailResponse_WhenGroupDoesNotExist()
    {
        // Arrange
        int groupId = 999;
        _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync((Group)null);

        // Act
        var result = await _userService.GetAllUsersFromGroup(groupId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Be("Group with this id not found");
    }

    [TestMethod]
    public async Task GetAllUsersFromGroup_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int groupId = 1;
        var group = new Group { ID = groupId, Name = "TestGroup" };
        _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync(group);
        _mockUserRepository.Setup(x => x.GetAllUsersFromGroup(groupId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.GetAllUsersFromGroup(groupId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region GetUsersWithoutGroup Tests

    [TestMethod]
    public async Task GetUsersWithoutGroup_ShouldReturnUsersWithoutGroups()
    {
        // Arrange
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Groups = new List<Group>(), Roles = new List<Role>() },
            new User { ID = 2, Name = "Jane", Email = "jane@test.com", Groups = new List<Group> { new Group { ID = 1 } }, Roles = new List<Role>() },
            new User { ID = 3, Name = "Bob", Email = "bob@test.com", Groups = new List<Group>(), Roles = new List<Role>() }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersWithoutGroup();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(u => u.ID == 1);
        result.Data.Should().Contain(u => u.ID == 3);
    }

    [TestMethod]
    public async Task GetUsersWithoutGroup_ShouldReturnEmptyList_WhenAllUsersHaveGroups()
    {
        // Arrange
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Groups = new List<Group> { new Group { ID = 1 } }, Roles = new List<Role>() }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersWithoutGroup();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    #endregion

    #region GetTeamLeadersWithoutGroup Tests

    [TestMethod]
    public async Task GetTeamLeadersWithoutGroup_ShouldReturnTeamLeadersWithoutGroup()
    {
        // Arrange
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role> { new Role { Role_Name = "TeamLeader" } } },
            new User { ID = 2, Name = "Jane", Email = "jane@test.com", Roles = new List<Role> { new Role { Role_Name = "Admin" } } }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetTeamLeadersWithoutGroup();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data[0].ID.Should().Be(1);
    }

    [TestMethod]
    public async Task GetTeamLeadersWithoutGroup_ShouldReturnFailResponse_WhenInvalidRoleProvided()
    {
        // Arrange
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role> { new Role { Role_Name = "InvalidRole" } } }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);
        
        // Change available roles to not include TeamLeader
        var roleConfig = new RoleConfiguration { AvailableRoles = [ "Admin" ] };
        _mockRoleOptions.Setup(x => x.Value).Returns(roleConfig);

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockGroupRepository.Object,
            _mockRoleOptions.Object);

        // Act
        var result = await _userService.GetTeamLeadersWithoutGroup();

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("TeamLeader");
    }

    [TestMethod]
    public async Task GetTeamLeadersWithoutGroup_ShouldReturnFail_WhenGetAllUsersFails()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.GetTeamLeadersWithoutGroup();

        // Assert
        result.Success.Should().BeFalse();
    }

    #endregion

    #region GetUserById Tests

    [TestMethod]
    public async Task GetUserById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        var user = new User { ID = userId, Name = "John", Email = "john@test.com", Roles = new List<Role>() };
        _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(userId);
        result.Data.Name.Should().Be("John");
    }

    [TestMethod]
    public async Task GetUserById_ShouldReturnFailResponse_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 999;
        _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [TestMethod]
    public async Task GetUserById_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int userId = 1;
        _mockUserRepository.Setup(x => x.GetUserById(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region GetUsersWithRole Tests

    [TestMethod]
    public async Task GetUsersWithRole_ShouldReturnUsersWithSpecifiedRole()
    {
        // Arrange
        string role = "TeamLeader";
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role> { new Role { Role_Name = "TeamLeader" } } },
            new User { ID = 2, Name = "Jane", Email = "jane@test.com", Roles = new List<Role> { new Role { Role_Name = "Admin" } } },
            new User { ID = 3, Name = "Bob", Email = "bob@test.com", Roles = new List<Role> { new Role { Role_Name = "TeamLeader" } } }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersWithRole(role);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().AllSatisfy(u => u.Roles.Should().Contain(role));
    }

    [TestMethod]
    public async Task GetUsersWithRole_ShouldReturnFailResponse_WhenRoleIsNotAvailable()
    {
        // Arrange
        string role = "InvalidRole";
        var users = new List<User>();
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersWithRole(role);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not available");
    }

    [TestMethod]
    public async Task GetUsersWithRole_ShouldReturnEmptyList_WhenNoUsersHaveRole()
    {
        // Arrange
        string role = "TeamLeader";
        var users = new List<User>
        {
            new User { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<Role> { new Role { Role_Name = "Admin" } } }
        };
        _mockUserRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersWithRole(role);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetUsersWithRole_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        string role = "TeamLeader";
        _mockUserRepository.Setup(x => x.GetAllUsers())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.GetUsersWithRole(role);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region UpdateUser Tests

    [TestMethod]
    public async Task UpdateUser_ShouldUpdateUserSuccessfully_WhenValidDataProvided()
    {
        // Arrange
        var dto = new UserUpdateDTO
        {
            ID = 1,
            Name = "John",
            Surname = "Doe",
            Email = "john.doe@test.com",
            Login = "johndoe",
            Nickname = "JD",
            Password = "newPassword",
            Roles = new List<string> { "User" }
        };

        var existingUser = new User
        {
            ID = 1,
            Name = "John",
            Email = "john@test.com",
            Salt = Encoding.UTF8.GetBytes("salt123"),
            Password = "oldPasswordHash",
            Roles = new List<Role>()
        };

        var roles = new List<Role> { new Role { Role_Name = "User" } };

        _mockUserRepository.Setup(x => x.GetUserById(dto.ID)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync((User)null);
        _mockUserRepository.Setup(x => x.GetUserByNickname(dto.Nickname)).ReturnsAsync((User)null);
        _mockRoleRepository.Setup(x => x.GetAllRoles()).ReturnsAsync(roles);
        _mockUserRepository.Setup(x => x.UpdateUser(It.IsAny<User>())).ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeTrue();
        _mockUserRepository.Verify(x => x.UpdateUser(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateUser_ShouldReturnFailResponse_WhenDtoIsNull()
    {
        // Act
        var result = await _userService.UpdateUser(null);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Be("Update data cannot be null");
    }

    [TestMethod]
    public async Task UpdateUser_ShouldReturnFailResponse_WhenUserDoesNotExist()
    {
        // Arrange
        var dto = new UserUpdateDTO { ID = 999, Name = "John", Email = "john@test.com" };
        _mockUserRepository.Setup(x => x.GetUserById(dto.ID)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [TestMethod]
    public async Task UpdateUser_ShouldReturnFailResponse_WhenEmailIsAlreadyTaken()
    {
        // Arrange
        var dto = new UserUpdateDTO
        {
            ID = 1,
            Name = "John",
            Email = "taken@test.com",
            Roles = new List<string>()
        };

        var existingUser = new User { ID = 1, Email = "old@test.com", Salt = Encoding.UTF8.GetBytes("salt") };
        var userWithEmail = new User { ID = 2, Email = "taken@test.com" };

        _mockUserRepository.Setup(x => x.GetUserById(dto.ID)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync(userWithEmail);

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Be("Email is already taken");
    }

    [TestMethod]
    public async Task UpdateUser_ShouldReturnFailResponse_WhenNicknameIsAlreadyTaken()
    {
        // Arrange
        var dto = new UserUpdateDTO
        {
            ID = 1,
            Name = "John",
            Email = "john@test.com",
            Nickname = "taken",
            Roles = new List<string>()
        };

        var existingUser = new User { ID = 1, Email = "john@test.com", Salt = Encoding.UTF8.GetBytes("salt") };
        var userWithNickname = new User { ID = 2, Nickname = "taken" };

        _mockUserRepository.Setup(x => x.GetUserById(dto.ID)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync((User)null);
        _mockUserRepository.Setup(x => x.GetUserByNickname(dto.Nickname)).ReturnsAsync(userWithNickname);

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Be("Nickname is already taken");
    }

    [TestMethod]
    public async Task UpdateUser_ShouldPreserveOldPassword_WhenNoNewPasswordProvided()
    {
        // Arrange
        var dto = new UserUpdateDTO
        {
            ID = 1,
            Name = "John",
            Email = "john@test.com",
            Roles = new List<string> { "User" }
        };

        var existingUser = new User
        {
            ID = 1,
            Email = "john@test.com",
            Salt = Encoding.UTF8.GetBytes("salt123"),
            Password = "oldPasswordHash",
            Roles = new List<Role>()
        };

        var roles = new List<Role> { new Role { Role_Name = "User" } };

        _mockUserRepository.Setup(x => x.GetUserById(dto.ID)).ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync((User)null);
        _mockUserRepository.Setup(x => x.GetUserByNickname(It.IsAny<string>())).ReturnsAsync((User)null);
        _mockRoleRepository.Setup(x => x.GetAllRoles()).ReturnsAsync(roles);
        _mockUserRepository.Setup(x => x.UpdateUser(It.IsAny<User>())).ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeTrue();
        _mockUserRepository.Verify(x => x.UpdateUser(It.Is<User>(u => u.Password == existingUser.Password)), Times.Once);
    }

    [TestMethod]
    public async Task UpdateUser_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        var dto = new UserUpdateDTO { ID = 1, Name = "John", Email = "john@test.com", Roles = new List<string>() };
        _mockUserRepository.Setup(x => x.GetUserById(dto.ID))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.UpdateUser(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region DeleteUser Tests

    [TestMethod]
    public async Task DeleteUser_ShouldDeleteUserSuccessfully_WhenUserExists()
    {
        // Arrange
        int userId = 1;
        _mockUserRepository.Setup(x => x.DeleteUser(userId)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUser(userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        _mockUserRepository.Verify(x => x.DeleteUser(userId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteUser_ShouldReturnFailResponse_WhenDeleteFails()
    {
        // Arrange
        int userId = 1;
        _mockUserRepository.Setup(x => x.DeleteUser(userId)).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUser(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Failed to delete");
    }

    [TestMethod]
    public async Task DeleteUser_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int userId = 1;
        _mockUserRepository.Setup(x => x.DeleteUser(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _userService.DeleteUser(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion
}
