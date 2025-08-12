using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.task;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule;
[ApiController]
[Route("[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTasks(int userId)
    {
        var response = await _taskService.GetUserTasks(userId);
    
        return response.Success 
            ? Ok(response.Data)
            : BadRequest(new { Message = response.Error }); 
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var response = await _taskService.GetTaskById(id);
    
        return response.Success 
            ? Ok(response.Data): BadRequest(new { Message = response.Error });
    }
    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetGroupTasks(int groupId)
    {
        var response = await _taskService.GetGroupTasks(groupId);
        
        return response.Success 
            ? Ok(response.Data)
            : BadRequest(new { Message = response.Error });
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddTask(CreateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.CreateTask(model);
        
        var task = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID}, "Added new Task with ID: " + new {id = task.ID})
            : BadRequest(new { Message = response.Error });
    }

    [HttpPost("add/user/{userId}")]
    public async Task<IActionResult> AddTaskUser(int userId, CreateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.CreateTaskAssignToUser(model, userId);
        
        var task = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID}, "Added new Task with ID: " + new {id = task.ID} + "\nAssigned User with id " + new {userId = userId} + " to the task")
            : BadRequest(new { Message = response.Error });
    }

    [HttpPost("add/group/{groupId}")]
    public async Task<IActionResult> AddTaskGroup(int groupId, CreateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.CreateTaskAssignToGroup(model, groupId);
        
        var task = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID}, "Added new Task with ID: " + new {id = task.ID} + "\nAssigned Group with id " + new {groupId = groupId} + " to the task")
            : BadRequest(new { Message = response.Error });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.UpdateTask(model, id);
        
        var task = response.Data;
        
        if (!response.Success)
            return response.Error.Equals("Task not found")
                ? NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error });
        
        return Ok(response.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var response = await _taskService.DeleteTask(id);

        if (!response.Success)
        {
            if (response.Error.Equals("Task not found")) return NotFound(new { Message = response.Error });
            if (response.Error.Equals("Task already deleted")) return Conflict(new { Message = response.Error });
            return BadRequest(new { Message = response.Error });
        }
        return NoContent();
    }
}