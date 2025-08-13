using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Models.DTOs;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule;

[ApiController]
[Route("[controller]")]
[Authorize]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;
    private readonly ITaskService _taskService;

    public StatusController(IStatusService statusService, ITaskService taskService)
    {
        _statusService = statusService;
        _taskService = taskService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddStatus(StatusDTO model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _statusService.AddStatus(model);

        var status = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetStatusById), new { id = status.ID }, $"Added new status with ID: {status.ID}")
            : BadRequest(new { Message = response.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStatus(StatusDTO model, int id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _statusService.UpdateStatus(model, id);
        
        if (!response.Success)
            return response.Error.Equals("Status not found")
                ? NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error });
        
        return Ok(response.Data);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStatusById(int id)
    {
        var response = await _statusService.GetStatusById(id);
        
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<IActionResult> GetTasksForStatus(int id)
    {
        var response = await _taskService.GetTasksByStatusId(id);

        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }
}