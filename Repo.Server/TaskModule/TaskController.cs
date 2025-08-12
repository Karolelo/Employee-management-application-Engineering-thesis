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
    
}