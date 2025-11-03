using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(policy: "TeamLeaderOnly")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsers();
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("group/{id}")]
    public async Task<IActionResult> GetUsersByGroupId([FromRoute] int id)
    {
        var response = await _userService.GetAllUsersFromGroup(id);
        if (response.Success == false && response.Error.StartsWith("Group with"))
        {
            return BadRequest(new { message = response.Error });
        }
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid user ID" });

        var response = await _userService.GetUserById(id);
        return response.Success
            ? Ok(response.Data) 
            : NotFound(new { message = response.Error });
    }

    [HttpGet("role")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] string role)
    {
        var response = await _userService.GetUsersWithRole(role);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _userService.UpdateUser(dto);
        return response.Success
            ? Ok(response.Data)
            : BadRequest(new { message = response.Error });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "Invalid user ID" });

        var response = await _userService.DeleteUser(id);
        return response.Success
            ? NoContent()
            : BadRequest(new { message = response.Error });
    }
}