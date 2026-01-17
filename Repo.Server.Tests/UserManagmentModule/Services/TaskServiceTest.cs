using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repo.Core.Models;
using Repo.Core.Models.task;
using Repo.Server.TaskModule;
using Repo.Server.TaskModule.interafaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Repo.Server.Tests.UserManagmentModule.Services;

[TestClass]
[TestSubject(typeof(TaskService))]
public class TaskServiceTest
{
    private Mock<ITaskRepository> _mockTaskRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IGroupRepository> _mockGroupRepository;
    private Mock<IPriorityRepository> _mockPriorityRepository;
    private Mock<IStatusRepository> _mockStatusRepository;
    private ITaskService _taskService;

    [TestInitialize]
    public void SetUp()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockGroupRepository = new Mock<IGroupRepository>();
        _mockPriorityRepository = new Mock<IPriorityRepository>();
        _mockStatusRepository = new Mock<IStatusRepository>();

        _taskService = new TaskService(
            _mockTaskRepository.Object,
            _mockUserRepository.Object,
            _mockGroupRepository.Object,
            _mockPriorityRepository.Object,
            _mockStatusRepository.Object);
    }

    #region GetTaskById Tests

    [TestMethod]
    public async Task GetTaskById_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        int taskId = 1;
        var task = new Repo.Core.Models.Task
        {
            ID = taskId,
            Name = "Test Task",
            Description = "Test Description",
            Priority = new Priority { ID = 1, Priority1 = "High" },
            Status = new Status { ID = 1, Status1 = "In Progress" }
        };
        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskById(taskId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(taskId);
        result.Data.Name.Should().Be("Test Task");
    }

    [TestMethod]
    public async Task GetTaskById_ShouldReturnFailResponse_WhenTaskDoesNotExist()
    {
        // Arrange
        int taskId = 999;
        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync((Repo.Core.Models.Task)null);

        // Act
        var result = await _taskService.GetTaskById(taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    #endregion

    #region GetTaskWithRelatedTasks Tests

    [TestMethod]
    public async Task GetTaskWithRelatedTasks_ShouldReturnTaskWithRelatedTasks_WhenTaskExists()
    {
        // Arrange
        int taskId = 1;
        var mainTask = new Repo.Core.Models.Task
        {
            ID = taskId,
            Name = "Main Task",
            Priority = new Priority { ID = 1, Priority1 = "High" },
            Status = new Status { ID = 1, Status1 = "In Progress" }
        };
        var relatedTask = new Repo.Core.Models.Task
        {
            ID = 2,
            Name = "Related Task",
            Priority = new Priority { ID = 1, Priority1 = "High" },
            Status = new Status { ID = 1, Status1 = "In Progress" }
        };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(mainTask);
        _mockTaskRepository.Setup(x => x.GetRelatedTaskIds(taskId)).ReturnsAsync(new List<int> { 2 });
        _mockTaskRepository.Setup(x => x.GetTaskById(2)).ReturnsAsync(relatedTask);

        // Act
        var result = await _taskService.GetTaskWithRelatedTasks(taskId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Task.ID.Should().Be(taskId);
        result.Data.RelatedTasks.Should().HaveCount(1);
        result.Data.RelatedTasks.First().ID.Should().Be(2);
    }

    [TestMethod]
    public async Task GetTaskWithRelatedTasks_ShouldReturnFailResponse_WhenTaskNotFound()
    {
        // Arrange
        int taskId = 999;
        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync((Repo.Core.Models.Task)null);

        // Act
        var result = await _taskService.GetTaskWithRelatedTasks(taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    #endregion

    #region GetUserTasks Tests

    [TestMethod]
    public async Task GetUserTasks_ShouldReturnUserTasks_WhenTasksExist()
    {
        // Arrange
        int userId = 1;
        var tasks = new List<Repo.Core.Models.Task>
        {
            new Repo.Core.Models.Task
            {
                ID = 1,
                Name = "Task 1",
                Priority = new Priority { ID = 1, Priority1 = "High" },
                Status = new Status { ID = 1, Status1 = "In Progress" }
            },
            new Repo.Core.Models.Task
            {
                ID = 2,
                Name = "Task 2",
                Priority = new Priority { ID = 1, Priority1 = "High" },
                Status = new Status { ID = 1, Status1 = "In Progress" }
            }
        };
        _mockTaskRepository.Setup(x => x.GetUserTasks(userId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetUserTasks(userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(t => t.ID == 1);
        result.Data.Should().Contain(t => t.ID == 2);
    }

    [TestMethod]
    public async Task GetUserTasks_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange
        int userId = 1;
        _mockTaskRepository.Setup(x => x.GetUserTasks(userId)).ReturnsAsync(new List<Repo.Core.Models.Task>());

        // Act
        var result = await _taskService.GetUserTasks(userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetUserTasks_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int userId = 1;
        _mockTaskRepository.Setup(x => x.GetUserTasks(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _taskService.GetUserTasks(userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region GetGroupTasks Tests

    [TestMethod]
    public async Task GetGroupTasks_ShouldReturnGroupTasks_WhenTasksExist()
    {
        // Arrange
        int groupId = 1;
        var tasks = new List<Repo.Core.Models.Task>
        {
            new Repo.Core.Models.Task
            {
                ID = 1,
                Name = "Task 1",
                Priority = new Priority { ID = 1, Priority1 = "High" },
                Status = new Status { ID = 1, Status1 = "In Progress" }
            }
        };
        _mockTaskRepository.Setup(x => x.GetGroupTasks(groupId)).ReturnsAsync(tasks);

        // Act
        var result = await _taskService.GetGroupTasks(groupId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data.First().ID.Should().Be(1);
    }

    [TestMethod]
    public async Task GetGroupTasks_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int groupId = 1;
        _mockTaskRepository.Setup(x => x.GetGroupTasks(groupId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _taskService.GetGroupTasks(groupId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region CreateTask Tests

    [TestMethod]
    public async Task CreateTask_ShouldCreateTask_WhenValidDataProvided()
    {
        // Arrange
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Description = "Description",
            Start_Time = DateTime.Now,
            Estimated_Time = 5,
            Priority = "High",
            Status = "In Progress",
            Creator_ID = 1
        };

        var priority = new Priority { ID = 1, Priority1 = "High" };
        var status = new Status { ID = 1, Status1 = "In Progress" };
        var createdTask = new Repo.Core.Models.Task
        {
            ID = 1,
            Name = model.Name,
            Description = model.Description,
            Priority = priority,
            Status = status
        };

        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("In Progress")).ReturnsAsync(status);
        _mockTaskRepository.Setup(x => x.CreateTask(It.IsAny<Repo.Core.Models.Task>())).ReturnsAsync(createdTask);

        // Act
        var result = await _taskService.CreateTask(model);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(1);
        result.Data.Name.Should().Be("New Task");
        _mockTaskRepository.Verify(x => x.CreateTask(It.IsAny<Repo.Core.Models.Task>()), Times.Once);
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnFailResponse_WhenPriorityIsEmpty()
    {
        // Arrange
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Priority = null,
            Status = "In Progress"
        };

        // Act
        var result = await _taskService.CreateTask(model);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Priority and Status cannot be empty");
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnFailResponse_WhenStatusNotFound()
    {
        // Arrange
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Priority = "High",
            Status = "InvalidStatus"
        };

        var priority = new Priority { ID = 1, Priority1 = "High" };
        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("InvalidStatus")).ReturnsAsync((Status)null);

        // Act
        var result = await _taskService.CreateTask(model);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Status with this name not exists");
    }

    [TestMethod]
    public async Task CreateTask_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Priority = "High",
            Status = "In Progress"
        };

        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _taskService.CreateTask(model);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region CreateTaskAssignToUser Tests

    [TestMethod]
    public async Task CreateTaskAssignToUser_ShouldCreateAndAssignTaskToUser_WhenValidDataProvided()
    {
        // Arrange
        int userId = 1;
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Description = "Description",
            Priority = "High",
            Status = "In Progress",
            Creator_ID = 1
        };

        var priority = new Priority { ID = 1, Priority1 = "High" };
        var status = new Status { ID = 1, Status1 = "In Progress" };
        var createdTask = new Repo.Core.Models.Task
        {
            ID = 1,
            Name = model.Name,
            Priority = priority,
            Status = status
        };

        _mockUserRepository.Setup(x => x.UserExists(userId)).ReturnsAsync(true);
        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("In Progress")).ReturnsAsync(status);
        _mockTaskRepository.Setup(x => x.CreateTask(It.IsAny<Repo.Core.Models.Task>())).ReturnsAsync(createdTask);
        _mockUserRepository.Setup(x => x.AddTaskToUser(userId, 1)).ReturnsAsync(true);

        // Act
        var result = await _taskService.CreateTaskAssignToUser(model, userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(1);
        _mockUserRepository.Verify(x => x.AddTaskToUser(userId, 1), Times.Once);
    }

    [TestMethod]
    public async Task CreateTaskAssignToUser_ShouldReturnFailResponse_WhenUserNotFound()
    {
        // Arrange
        int userId = 999;
        var model = new CreateTaskModel { Name = "New Task" };

        _mockUserRepository.Setup(x => x.UserExists(userId)).ReturnsAsync(false);

        // Act
        var result = await _taskService.CreateTaskAssignToUser(model, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("User not found");
    }

    #endregion

    #region CreateTaskAssignToGroup Tests

    [TestMethod]
    public async Task CreateTaskAssignToGroup_ShouldCreateAndAssignTaskToGroup_WhenValidDataProvided()
    {
        // Arrange
        int groupId = 1;
        var model = new CreateTaskModel
        {
            Name = "New Task",
            Priority = "High",
            Status = "In Progress"
        };

        var group = new Group { ID = groupId, Name = "TestGroup" };
        var priority = new Priority { ID = 1, Priority1 = "High" };
        var status = new Status { ID = 1, Status1 = "In Progress" };
        var createdTask = new Repo.Core.Models.Task
        {
            ID = 1,
            Name = model.Name,
            Priority = priority,
            Status = status
        };

        _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync(group);
        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("In Progress")).ReturnsAsync(status);
        _mockTaskRepository.Setup(x => x.CreateTask(It.IsAny<Repo.Core.Models.Task>())).ReturnsAsync(createdTask);
        _mockGroupRepository.Setup(x => x.AddTaskToGroup(groupId, 1)).ReturnsAsync(true);

        // Act
        var result = await _taskService.CreateTaskAssignToGroup(model, groupId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(1);
        _mockGroupRepository.Verify(x => x.AddTaskToGroup(groupId, 1), Times.Once);
    }

    [TestMethod]
    public async Task CreateTaskAssignToGroup_ShouldReturnFailResponse_WhenGroupNotFound()
    {
        // Arrange
        int groupId = 999;
        var model = new CreateTaskModel { Name = "New Task" };

        _mockGroupRepository.Setup(x => x.GetGroupById(groupId)).ReturnsAsync((Group)null);

        // Act
        var result = await _taskService.CreateTaskAssignToGroup(model, groupId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Group");
    }

    #endregion

    #region UpdateTask Tests

    [TestMethod]
    public async Task UpdateTask_ShouldUpdateTask_WhenValidDataProvided()
    {
        // Arrange
        int taskId = 1;
        var dto = new UpdateTaskDTO
        {
            Name = "Updated Task",
            Description = "Updated Description",
            Priority = "High",
            Status = "Completed"
        };

        var existingTask = new Repo.Core.Models.Task
        {
            ID = taskId,
            Name = "Old Task",
            Priority = new Priority { ID = 1, Priority1 = "Low" },
            Status = new Status { ID = 1, Status1 = "In Progress" }
        };

        var priority = new Priority { ID = 1, Priority1 = "High" };
        var status = new Status { ID = 2, Status1 = "Completed" };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("Completed")).ReturnsAsync(status);
        _mockTaskRepository.Setup(x => x.UpdateTask(It.IsAny<Repo.Core.Models.Task>())).ReturnsAsync(existingTask);

        // Act
        var result = await _taskService.UpdateTask(dto, taskId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Name.Should().Be("Updated Task");
        _mockTaskRepository.Verify(x => x.UpdateTask(It.IsAny<Repo.Core.Models.Task>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateTask_ShouldReturnFailResponse_WhenTaskNotFound()
    {
        // Arrange
        int taskId = 999;
        var dto = new UpdateTaskDTO { Name = "Updated Task" };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync((Repo.Core.Models.Task)null);

        // Act
        var result = await _taskService.UpdateTask(dto, taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    [TestMethod]
    public async Task UpdateTask_ShouldReturnFailResponse_WhenConcurrencyConflict()
    {
        // Arrange
        int taskId = 1;
        var dto = new UpdateTaskDTO { Name = "Updated Task", Priority = "High", Status = "In Progress" };
        var existingTask = new Repo.Core.Models.Task { ID = taskId };
        var priority = new Priority { ID = 1, Priority1 = "High" };
        var status = new Status { ID = 1, Status1 = "In Progress" };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(existingTask);
        _mockPriorityRepository.Setup(x => x.GetPriorityByName("High")).ReturnsAsync(priority);
        _mockStatusRepository.Setup(x => x.GetStatusByName("In Progress")).ReturnsAsync(status);
        _mockTaskRepository.Setup(x => x.UpdateTask(It.IsAny<Repo.Core.Models.Task>()))
            .ThrowsAsync(new DbUpdateConcurrencyException("Concurrency conflict", new Exception()));

        // Act
        var result = await _taskService.UpdateTask(dto, taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Concurrency conflict");
    }

    #endregion

    #region DeleteTask Tests

    [TestMethod]
    public async Task DeleteTask_ShouldDeleteTask_WhenTaskExists()
    {
        // Arrange
        int taskId = 1;
        var task = new Repo.Core.Models.Task { ID = taskId, Name = "Task to Delete" };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(task);
        _mockTaskRepository.Setup(x => x.DeleteTask(taskId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.DeleteTask(taskId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.ID.Should().Be(taskId);
        _mockTaskRepository.Verify(x => x.DeleteTask(taskId), Times.Once);
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnFailResponse_WhenTaskNotFound()
    {
        // Arrange
        int taskId = 999;
        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync((Repo.Core.Models.Task)null);

        // Act
        var result = await _taskService.DeleteTask(taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Task not found");
    }

    [TestMethod]
    public async Task DeleteTask_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int taskId = 1;
        var task = new Repo.Core.Models.Task { ID = taskId };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(task);
        _mockTaskRepository.Setup(x => x.DeleteTask(taskId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _taskService.DeleteTask(taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    #region AddRelation Tests

    [TestMethod]
    public async Task AddRelation_ShouldAddRelation_WhenValidTaskIdsProvided()
    {
        // Arrange
        int taskId = 1;
        int relatedTaskId = 2;

        var task1 = new Repo.Core.Models.Task { ID = taskId };
        var task2 = new Repo.Core.Models.Task { ID = relatedTaskId };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(task1);
        _mockTaskRepository.Setup(x => x.GetTaskById(relatedTaskId)).ReturnsAsync(task2);
        _mockTaskRepository.Setup(x => x.RelationExists(taskId, relatedTaskId)).ReturnsAsync(false);
        _mockTaskRepository.Setup(x => x.AddRelation(taskId, relatedTaskId)).ReturnsAsync(new RelatedTask());

        // Act
        var result = await _taskService.AddRelation(taskId, relatedTaskId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.MainTaskID.Should().Be(taskId);
        result.Data.RelatedTaskID.Should().Be(relatedTaskId);
        _mockTaskRepository.Verify(x => x.AddRelation(taskId, relatedTaskId), Times.Once);
    }

    [TestMethod]
    public async Task AddRelation_ShouldReturnFailResponse_WhenTaskIdEqualsRelatedTaskId()
    {
        // Arrange
        int taskId = 1;

        // Act
        var result = await _taskService.AddRelation(taskId, taskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Cannot relate task to itself");
    }

    [TestMethod]
    public async Task AddRelation_ShouldReturnFailResponse_WhenRelationAlreadyExists()
    {
        // Arrange
        int taskId = 1;
        int relatedTaskId = 2;

        var task1 = new Repo.Core.Models.Task { ID = taskId };
        var task2 = new Repo.Core.Models.Task { ID = relatedTaskId };

        _mockTaskRepository.Setup(x => x.GetTaskById(taskId)).ReturnsAsync(task1);
        _mockTaskRepository.Setup(x => x.GetTaskById(relatedTaskId)).ReturnsAsync(task2);
        _mockTaskRepository.Setup(x => x.RelationExists(taskId, relatedTaskId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.AddRelation(taskId, relatedTaskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Relation already exists");
    }

    #endregion

    #region RemoveRelation Tests

    [TestMethod]
    public async Task RemoveRelation_ShouldRemoveRelation_WhenRelationExists()
    {
        // Arrange
        int taskId = 1;
        int relatedTaskId = 2;

        _mockTaskRepository.Setup(x => x.RelationExists(taskId, relatedTaskId)).ReturnsAsync(true);
        _mockTaskRepository.Setup(x => x.RemoveRelation(taskId, relatedTaskId)).ReturnsAsync(true);

        // Act
        var result = await _taskService.RemoveRelation(taskId, relatedTaskId);

        // Assert
        result.Success.Should().BeTrue();
        _mockTaskRepository.Verify(x => x.RemoveRelation(taskId, relatedTaskId), Times.Once);
    }

    [TestMethod]
    public async Task RemoveRelation_ShouldReturnFailResponse_WhenRelationDoesNotExist()
    {
        // Arrange
        int taskId = 1;
        int relatedTaskId = 2;

        _mockTaskRepository.Setup(x => x.RelationExists(taskId, relatedTaskId)).ReturnsAsync(false);

        // Act
        var result = await _taskService.RemoveRelation(taskId, relatedTaskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Relation not found");
    }

    [TestMethod]
    public async Task RemoveRelation_ShouldReturnFailResponse_WhenExceptionOccurs()
    {
        // Arrange
        int taskId = 1;
        int relatedTaskId = 2;

        _mockTaskRepository.Setup(x => x.RelationExists(taskId, relatedTaskId)).ReturnsAsync(true);
        _mockTaskRepository.Setup(x => x.RemoveRelation(taskId, relatedTaskId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _taskService.RemoveRelation(taskId, relatedTaskId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Database error");
    }

    #endregion

    
}