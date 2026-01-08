using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TeamLeader")]
public class UserGradeController : ControllerBase
{
    private readonly IUserGradeService _userGradeService;

    public UserGradeController(IUserGradeService userGradeService)
    {
        _userGradeService = userGradeService;
    }
    
    //[HttpGet] methods
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] string? q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await _userGradeService.GetUsers(q, page, pageSize);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var response = await _userGradeService.GetUserById(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }
}