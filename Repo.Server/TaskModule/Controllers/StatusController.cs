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

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
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
        throw new NotImplementedException();
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStatusById(int id)
    {
        throw new NotImplementedException();
    }
}