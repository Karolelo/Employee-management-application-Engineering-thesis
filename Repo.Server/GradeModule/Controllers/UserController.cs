using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TeamLeader")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    //[HttpGet] methods
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _userService.GetUsers(q, page, pageSize);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var response = await _userService.GetUserById(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }
}