using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repo.Core.Infrastructure.Files;
using Repo.Core.Models;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;
using Repo.Server.UserManagmentModule.Services;

using Group = Repo.Core.Models.Group;
using Task = System.Threading.Tasks.Task;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
[TestSubject(typeof(GroupServiceTest))]
public class GroupServiceTest
{
   private Mock<IGroupRepository> _mockGroupRepository;
   private Mock<IFileOperations> _mockFile;
   private Mock<IUserRepository> _mockUserRepository;
   private IGroupService _groupService;

   [TestInitialize]
   public void SetUp()
   {
      _mockGroupRepository = new Mock<IGroupRepository>();
      _mockFile = new Mock<IFileOperations>();
      _mockUserRepository = new Mock<IUserRepository>();

      _groupService = new GroupService(_mockGroupRepository.Object, _mockFile.Object, _mockUserRepository.Object);
   }


   #region GetAllGroups Tests

   [TestMethod]
   public async Task GetAllGroups_ShouldReturnGroups_WhenGroupsExist()
   {
      // Arrange
      var groups = new List<Group>()
      {
         new Group {ID = 1,Name = "Software Developers",Admin_ID = 1,Deleted = 0,Description = "Software developers team"},
         new Group {ID = 2,Name = "HR",Admin_ID = 2,Deleted = 0,Description = "People which hiring our team"}
      };

      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      
      // Act
      var result = await _groupService.GetAllGroups();

      // Assert
      result.Success.Should().BeTrue();
      result.Data.Should().HaveCount(2);
      result.Data[0].Name.Should().Be("Software Developers");
      result.Data[1].Name.Should().Be("HR");
   }

   [TestMethod]
   public async Task GetAllGroups_ShouldReturnEmptyList_WhenGroupsNotExist()
   {
      // Arrange
      var groups = new List<Group>();
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      
      // Act
      var result = await _groupService.GetAllGroups();

      // Assert
      result.Success.Should().BeTrue();
      result.Data.Should().BeEmpty();
   }

   [TestMethod]
   public async Task GetAllGroups_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      _mockGroupRepository.Setup(x => x.GetAllGroups())
         .Throws(new Exception("Database error"));
      
      // Act
      var result = await _groupService.GetAllGroups();
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }
   
   #endregion

   #region GetGroupById Tests

   [TestMethod]
   public async Task GetGroupById_ShouldReturnGroup_WhenGroupExists()
   {
      // Arrange
      int groupId = 1;
      var group = new Group
         { ID = groupId, Name = "Software Developers", Admin_ID = 1, Deleted = 0, Description = "Software developers team" };
      
      _mockGroupRepository.Setup(x => x.GetGroupById(1)).ReturnsAsync(group);
      
      // Act
      var result = await _groupService.GetGroupById(1);

      // Assert
      result.Success.Should().BeTrue();
      result.Data.ID.Should().Be(1);
      result.Data.Name.Should().Be("Software Developers");
   }
   
   [TestMethod]
   public async Task GetGroupById_ShouldReturnFailRespond_WhenGroupNotExists()
   {
      // Arrange
      int groupId = 1;
      var group = new Group
         { ID = groupId, Name = "Software Developers", Admin_ID = 1, Deleted = 0, Description = "Software developers team" };
      
      _mockGroupRepository.Setup(x => x.GetGroupById(1)).ReturnsAsync(group);
      
      // Act
      var result = await _groupService.GetGroupById(2);

      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not founded");
   }
   
   [TestMethod]
   public async Task GetGroupById_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      _mockGroupRepository.Setup(x => x.GetGroupById(1)).ThrowsAsync(new Exception("Database error"));
      
      // Act
      var result = await _groupService.GetGroupById(1);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }
   
   #endregion

   #region GetGroupByAdminId

   [TestMethod]
   public async Task GetGroupByAdminId_ShouldReturnGroup_WhenGroupExists()
   {
      // Arrange
      int adminId = 3;
      var groups = new List<Group>()
      {
         new Group {ID = 1,Name = "Software Developers",Admin_ID = 1,Deleted = 0,Description = "Software developers team"},
         new Group {ID = 2,Name = "HR",Admin_ID = adminId,Deleted = 0,Description = "People which hiring our team"}
      };
      
      var user = new User() { ID = 3, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      _mockUserRepository.Setup(x => x.GetUserById(adminId)).ReturnsAsync(user);
      
      // Act
      var result = await _groupService.GetGroupByAdminId(adminId);

      // Assert
      result.Success.Should().BeTrue();
      result.Data.ID.Should().Be(2);
      result.Data.Name.Should().Be("HR");
   }
   
   [TestMethod]
   public async Task GetGroupByAdminId_ShouldReturnFailRespond_WhenGroupNotExists()
   {
      // Arrange
      var adminId = 3;
      var groups = new List<Group>()
      {
         new Group {ID = 1,Name = "Software Developers",Admin_ID = 1,Deleted = 0,Description = "Software developers team"},
         new Group {ID = 2,Name = "HR",Admin_ID = 2,Deleted = 0,Description = "People which hiring our team"}
      };

      var user = new User() { ID = adminId, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      _mockUserRepository.Setup(x => x.GetUserById(adminId)).ReturnsAsync(user);
      
      // Act
      var result = await _groupService.GetGroupByAdminId(adminId);

      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not founded");
   }
   
   [TestMethod]
   public async Task GetGroupByAdminId_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      int adminId = 1;
      var user = new User() { ID = adminId, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ThrowsAsync(new Exception("Database error"));
      _mockUserRepository.Setup(x => x.GetUserById(adminId)).ReturnsAsync(user);
      // Act
      var result = await _groupService.GetGroupByAdminId(adminId);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }

   [TestMethod]
   public async Task GetGroupByAdminId_ShouldReturnFailResponse_WhenAdminNotExists()
   {
      // Arrange
      int adminId = 1;
      _mockUserRepository.Setup(x => x.GetUserById(adminId)).ReturnsAsync(null as User);
      
      // Act
      var result = await _groupService.GetGroupByAdminId(adminId);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not exists");
   }

   #endregion

   #region GetUsersGroups Tests

   [TestMethod]
   public async Task GetUsersGroups_ShouldReturnGroupsLists_WhenGroupsExist()
   {
      // Arrange
      int usersId = 1;
      var groups = new List<Group>()
      {
         new Group {ID = 1,Name = "Software Developers",Admin_ID = 1,Deleted = 0,Description = "Software developers team"},
         new Group {ID = 2,Name = "HR",Admin_ID = 2,Deleted = 0,Description = "People which hiring our team",Users = new List<User>()}
      };
      
      var user = new User() { ID = usersId, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      groups[0].Users.Add(user);
      groups[1].Users.Add(user);
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      _mockUserRepository.Setup(x => x.GetUserById(usersId)).ReturnsAsync(user);
      
      // Act
      var result = await _groupService.GetUsersGroups(usersId);

      // Assert
      result.Success.Should().BeTrue();
      result.Data[0].ID.Should().Be(1);
      result.Data[1].Name.Should().Be("HR");
   }
   
   [TestMethod]
   public async Task GetUsersGroups_ShouldReturnEmptyList_WhenGroupNotExists()
   {
      // Arrange
      var userId = 3;
      var groups = new List<Group>()
      {
         new Group {ID = 1,Name = "Software Developers",Admin_ID = 1,Deleted = 0,Description = "Software developers team"},
         new Group {ID = 2,Name = "HR",Admin_ID = 2,Deleted = 0,Description = "People which hiring our team"}
      };

      var user = new User() { ID = userId, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
      _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);
      
      // Act
      var result = await _groupService.GetUsersGroups(userId);

      // Assert
      result.Success.Should().BeTrue();
      result.Data.Should().BeEmpty();
   }
   
   [TestMethod]
   public async Task GetUsersGroups_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      int userId = 1;
      var user = new User() { ID = userId, Name = "Karol", Surname = "Kraszi", Nickname = "Blalal" };
      
      _mockGroupRepository.Setup(x => x.GetAllGroups()).ThrowsAsync(new Exception("Database error"));
      _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(user);
      // Act
      var result = await _groupService.GetUsersGroups(userId);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }

   [TestMethod]
   public async Task GetUsersGroups_ShouldReturnFailResponse_WhenUserNotExists()
   {
      // Arrange
      int userId = 1;
      _mockUserRepository.Setup(x => x.GetUserById(userId)).ReturnsAsync(null as User);
      
      // Act
      var result = await _groupService.GetUsersGroups(userId);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not exists");
   }

   #endregion
   
   #region CreateGroup Tests

   [TestMethod]
   public async Task CreateGroup_ShouldReturnGroup_WhenGroupIsCreated()
   {
      // Arrange
      var createGroupDTO = new CreateGroupDTO 
      { 
         Name = "New Team", 
         Admin_ID = 1, 
         Description = "New team description" 
      };
      
      var createdGroup = new Group 
      { 
         ID = 3, 
         Name = "New Team", 
         Admin_ID = 1, 
         Deleted = 0, 
         Description = "New team description" 
      };
      
      _mockGroupRepository.Setup(x => x.CreateGroup(It.IsAny<Group>())).ReturnsAsync(createdGroup);
      
      // Act
      var result = await _groupService.CreateGroup(createGroupDTO);
      
      // Assert
      result.Success.Should().BeTrue();
      result.Data.Name.Should().Be("New Team");
      result.Data.Admin_ID.Should().Be(1);
   }

   [TestMethod]
   public async Task CreateGroup_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      var createGroupDTO = new CreateGroupDTO  { Name = "New Team", Admin_ID = 1 };
      
      _mockGroupRepository.Setup(x => x.CreateGroup(It.IsAny<Group>()))
         .ThrowsAsync(new Exception("Database error"));
      
      // Act
      var result = await _groupService.CreateGroup(createGroupDTO);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }

   #endregion

   #region UpdateGroup Tests

   [TestMethod]
   public async Task UpdateGroup_ShouldReturnUpdatedGroup_WhenGroupExists()
   {
      // Arrange
      var updateGroupDTO = new UpdateGroupDTO
      { 
         ID = 1, 
         Name = "Updated Team", 
         Description = "Updated description" 
      };
      
      var updatedGroup = new Group 
      { 
         ID = 1, 
         Name = "Updated Team", 
         Admin_ID = 1, 
         Deleted = 0, 
         Description = "Updated description" 
      };
      
      _mockGroupRepository.Setup(x => x.GetGroupById(1)).ReturnsAsync(updatedGroup);
      _mockGroupRepository.Setup(x => x.UpdateGroup(It.IsAny<Group>())).ReturnsAsync(updatedGroup);
      
      // Act
      var result = await _groupService.UpdateGroup(updateGroupDTO);
      
      // Assert
      result.Success.Should().BeTrue();
      result.Data.Name.Should().Be("Updated Team");
   }

   [TestMethod]
   public async Task UpdateGroup_ShouldReturnFailResponse_WhenGroupNotExists()
   {
      // Arrange
      var updateGroupDTO = new UpdateGroupDTO { ID = 999, Name = "Updated Team" };
      
      _mockGroupRepository.Setup(x => x.GetGroupById(999)).ReturnsAsync(null as Group);
      
      // Act
      var result = await _groupService.UpdateGroup(updateGroupDTO);
      
      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not exists");
   }

   #endregion

   #region DeleteGroup Tests

   [TestMethod]
   public async Task DeleteGroup_ShouldReturnTrue_WhenGroupIsDeleted()
   {
      // Arrange
      int groupId = 1;
      _mockGroupRepository.Setup(x => x.DeleteGroup(groupId)).ReturnsAsync(true);
      
      // Act
      var result = await _groupService.DeleteGroup(groupId);
      
      // Assert
      result.Success.Should().BeTrue();
      result.Data.Should().BeTrue();
   }

   [TestMethod]
   public async Task DeleteGroup_ShouldReturnFailResponse_WhenGroupNotExists()
   {
      // Arrange
      int groupId = 999;
      _mockGroupRepository.Setup(x => x.DeleteGroup(groupId)).ReturnsAsync(false);
      
      // Act
      var result = await _groupService.DeleteGroup(groupId);
      
      // Assert
      result.Success.Should().BeFalse();
   }

   #endregion

   #region Additional test methods

   private Mock<IFromFile> CreateMockFile(string content, string fileName)

   #endregion
}