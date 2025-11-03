using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.user.annoucement;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Controllers;
[ApiController]
[Route("api/[controller]/group")]
[Authorize(policy:"TeamLeaderOnly")]
public class AnnouncementController :ControllerBase
{
    private readonly IAnnoucementService _annoucementService;

    public AnnouncementController(IAnnoucementService annoucementService)
    {
        _annoucementService = annoucementService;
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetGroupAnnouncements(int groupId)
    {
        var result = await _annoucementService.GetAllAnnouncements(groupId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpPost("")]
    public async Task<IActionResult> AddGroupAnnouncements(CreateAnnoucementDTO annoucement)
    {
        var result = await _annoucementService.AddAnnouncement(annoucement);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpPut("")] 
    public async Task<IActionResult> UpdateGroupAnnouncements(UpdateAnnoucementDTO annoucement)
    {
        var result = await _annoucementService.UpdateAnnouncement(annoucement);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteGroupAnnouncements(int Id)
    {
        var result = await _annoucementService.DeleteAnnouncement(Id);
        return result.Success ? NoContent() : BadRequest(result.Error);
    }
}