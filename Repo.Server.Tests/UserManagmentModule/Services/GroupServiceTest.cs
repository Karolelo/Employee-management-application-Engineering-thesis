using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
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

   #region GetGroupByAdminId Tests

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

   #region GetGroupImagePath Tests

   [TestMethod]
   public async Task GetGroupImagePath_ShouldReturnPath_WhenGroupIDExists()
   {
      //Arrange
      var pathToImage = "C://pliki/plik.txt";
      var groupId = 1;
      _mockGroupRepository.Setup(x => x.GetPathToImageFile(groupId)).ReturnsAsync(pathToImage);
      
      //Act
      var result = await _groupService.GetGroupImagePath(groupId);
      
      //Assertion
      result.Data.Should().Be(pathToImage);
   }
   
   [TestMethod]
   public async Task GetGroupImagePath_ShouldReturnFailResponse_WhenGroupDoesNotHaveImage()
   {
      //Arrange
      var groupId = 1;
      _mockGroupRepository.Setup(x => x.GetPathToImageFile(groupId)).ReturnsAsync("");
      
      //Act
      var result = await _groupService.GetGroupImagePath(groupId);
      
      //Assertion
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Group does not");
   }

   [TestMethod]
   public async Task GetGroupImagePath_ShouldReturnFailResponse_WhenErrorOccured()
   {
      var groupId = 1;
      _mockGroupRepository.Setup(x => x.GetPathToImageFile(groupId)).Throws(new Exception("database error"));
      
      //Act
      var result = await _groupService.GetGroupImagePath(groupId);
      
      //Assertion
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("database error");
   }

   #endregion

   #region Additional test methods

   #region SaveGroupImage Tests

   [TestMethod]
   public async Task SaveGroupImage_ShouldSaveFile_WhenGroupExists()
   {
      // Arrange
      var groupId = 1;
      var content = "JasioIStatsioLubiaSpiewacWDomuPiosenki";
      var fileName = "testImage.jpg";
      var file = CreateMockFile(content, fileName);
      var group = new Group { ID = groupId, Name = "Test Group" };

      _mockGroupRepository.Setup(x => x.GetGroupById(groupId))
         .ReturnsAsync(group);
      _mockFile.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>()))
         .Verifiable();

      // Act
      var result = await _groupService.SaveGroupImage(groupId, file.Object);

      // Assert
      result.Success.Should().BeTrue();
      result.Data.Should().Contain("group_1_");
      result.Data.Should().EndWith(".jpg");
      _mockFile.Verify(x => x.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
      _mockGroupRepository.Verify(x => x.SavePathToImageFile(groupId, It.IsAny<string>()), Times.Once);
   }

   [TestMethod]
   public async Task UpdateGroupImage_ShouldSaveFile_WhenGroupExists()
   {
      //Arrange
      var groupId = 1;
      var content = "JasioIStatsioLubiaSpiewacWDomuPiosenki";
      var fileName = "testImage.jpg";
      var file = CreateMockFile(content, fileName);
      var group = new Group { ID = groupId, Name = "Test Group" };
      var examplePath = "C://Files/files.jpg";
      
      _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync(group);
      _mockFile.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>())).Verifiable();
      _mockGroupRepository.Setup(x => x.GetPathToImageFile(groupId)).ReturnsAsync(examplePath).Verifiable();
      _mockGroupRepository.Setup(x => x.UpdateImageFile(groupId, It.IsAny<string>())).ReturnsAsync(examplePath).Verifiable();
      
      //Act
      var result = await _groupService.SaveGroupImage(groupId, file.Object, true);
      
      //Assert
      result.Success.Should().BeTrue();
      result.Data.Should().Contain("group_1_");
      result.Data.Should().EndWith(".jpg");
      _mockFile.Verify(x=>x.SaveFile(It.IsAny<string>(), It.IsAny<byte[]>()),Times.Once);
      _mockGroupRepository.Verify(x => x.UpdateImageFile(groupId,It.IsAny<string>()),Times.Once);
      _mockGroupRepository.Verify(x=>x.GetPathToImageFile(groupId),Times.Once);
   }
   
   [TestMethod]
   public async Task SaveGroupImage_ShouldReturnFailResponse_WhenGroupNotExists()
   {
      // Arrange
      var groupId = 999;
      var file = CreateMockFile("content", "test.jpg");

      _mockGroupRepository.Setup(x => x.GetGroupById(groupId))
         .ReturnsAsync(null as Group);

      // Act
      var result = await _groupService.SaveGroupImage(groupId, file.Object);

      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("not found");
   }

   [TestMethod]
   public async Task SaveGroupImage_ShouldReturnFailResponse_WhenExceptionOccurs()
   {
      // Arrange
      var groupId = 1;
      var file = CreateMockFile("content", "test.jpg");

      _mockGroupRepository.Setup(x => x.GetGroupById(groupId))
         .ThrowsAsync(new Exception("Database error"));

      // Act
      var result = await _groupService.SaveGroupImage(groupId, file.Object);

      // Assert
      result.Success.Should().BeFalse();
      result.Error.Should().Contain("Database error");
   }
   
   #endregion

   private Mock<IFormFile> CreateMockFile(string content, string fileName)
   {
      var mock = new Mock<IFormFile>();
      var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

      mock.Setup(x => x.OpenReadStream()).Returns(stream);
      mock.Setup(x => x.FileName).Returns(fileName);
      mock.Setup(x => x.Length).Returns(stream.Length);
      mock.Setup(x => x.ContentType).Returns("text/plain");
   
      return mock;
   }

   #endregion
}