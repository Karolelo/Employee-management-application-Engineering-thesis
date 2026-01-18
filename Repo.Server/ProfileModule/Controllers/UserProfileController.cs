using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.profile;

namespace Repo.Server.ProfileModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public UserProfileController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst("id")?.Value;
        return int.Parse(idClaim!);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        var response = await _profileService.GetProfileAsync(userId);

        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    [HttpPut("me/email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDTO model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetUserId();
        var response = await _profileService.ChangeEmailAsync(userId, model);

        return response.Success
            ? Ok(response.Data)
            : BadRequest(new { Message = response.Error });
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetUserId();
        var response = await _profileService.ChangePasswordAsync(userId, model);

        return response.Success
            ? Ok(response.Data)
            : BadRequest(new { Message = response.Error });
    }
}
