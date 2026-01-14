using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.DTOs;
using Repo.Core.Models.task;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    //Methods for getting task
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        var response = await _taskService.GetTaskById(id);
    
        return response.Success 
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    [HttpGet("{id:int}/relations", Name = "GetTaskWithRelated")]
    public async Task<IActionResult> GetTaskWithRelatedTasks(int id)
    {
        var response = await _taskService.GetTaskWithRelatedTasks(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }
    
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserTasks(int userId)
    {
        var response = await _taskService.GetUserTasks(userId);

        return response.Success ? Ok(response.Data) : Ok(new List<TaskDTO>());
    }

    [HttpGet("group/{groupId:int}")]
    public async Task<IActionResult> GetGroupTasks(int groupId)
    {
        var response = await _taskService.GetGroupTasks(groupId);
        
        return response.Success ? Ok(response.Data) : Ok(new List<TaskDTO>());
    }
    
    [HttpGet("user/{userId:int}/gantt")]
    public async Task<IActionResult> GetGanttTasks(int userId)
    {
        var response = await _taskService.GetGanttTasks(userId);

        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }


    //Methods for creating task
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
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID}, task)
            : BadRequest(new { Message = response.Error });
    }

    [HttpPost("user/add/{userId:int}")]
    public async Task<IActionResult> AddTaskUser(int userId, CreateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.CreateTaskAssignToUser(model, userId);
        
        var task = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID}, task)
            : BadRequest(new { Message = response.Error });
    }

    [HttpPost("group/add/{groupId:int}")]
    public async Task<IActionResult> AddTaskGroup(int groupId, CreateTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.CreateTaskAssignToGroup(model, groupId);
        
        var task = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetTaskById), new {id = task.ID},task)
            : BadRequest(new { Message = response.Error });
    }

    //Methods for updating task
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _taskService.UpdateTask(dto, id);
        
        var task = response.Data;
        
        if (!response.Success)
            return response.Error.Equals("Task not found")
                ? NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error });
        
        return Ok(response.Data);
    }

    //Methods for deleting task
    [HttpDelete("{id:int}")]
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

    //Methods for managing relations
    [HttpPost("{id:int}/relations")]
    public async Task<IActionResult> AddRelation(int id, CreateTaskRelationDTO model)
    {
        var response = await _taskService.AddRelation(id, model.RelatedTaskID);
        if (!response.Success)
        {
            return response.Error switch
            {
                "Task not found"                => NotFound(new { Message = response.Error }),
                "Cannot relate task to itself"  or
                "Relation already exists"       => Conflict(new { Message = response.Error }),
                _                               => BadRequest(new { Message = response.Error })
            };
        }

        return CreatedAtRoute("GetTaskWithRelated", new { id }, response.Data);
    }
    
    [HttpDelete("{id:int}/relations/{relatedId:int}")]
    public async Task<IActionResult> RemoveRelation(int id, int relatedId)
    {
        var response = await _taskService.RemoveRelation(id, relatedId);
        if (!response.Success)
            return response.Error.Equals("Relation not found")
                ?  NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error });
        
        return NoContent();
    }
}