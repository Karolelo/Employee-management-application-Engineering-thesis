using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TargetController : ControllerBase
{
    private readonly ITargetService _targetService;

    public TargetController(ITargetService targetService)
    {
        _targetService = targetService;
    }
    
    //[HttpGet] methods
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTargetById(int id)
    {
        var response = await _targetService.GetTargetById(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserTargets(int userId)
    {
        var response = await _targetService.GetUserTargets(userId);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }
}