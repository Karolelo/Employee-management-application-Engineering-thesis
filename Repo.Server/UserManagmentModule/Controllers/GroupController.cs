using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
        var response = await groupService.GetAllGroups();
        
        if (!response.Success)
            return NotFound(new { Message = response.Error });
            
        return Ok(response.Data);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGroupById([FromRoute]int id)
    {
        var response = await groupService.GetGroupById(id);
        
        if (!response.Success)
            return NotFound(new { Message = response.Error });
            
        return Ok(response.Data);
    }
    
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserGroups([FromRoute] int userId)
    {
        var response = await groupService.GetUsersGroups(userId);

        if (!response.Success)
        {
            return response.Error.Contains("not exists") 
                ? NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error }); 
        }

        return Ok(response.Data);
    }
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody]CreateGroupDTO model)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
            
        var response = await groupService.CreateGroup(model);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
            
        var group = response.Data;
        return CreatedAtAction(nameof(GetGroupById), new { id = group.ID }, group);
    }
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpPut]
    public async Task<IActionResult> UpdateGroup([FromBody]UpdateGroupDTO model)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
            
        var response = await groupService.UpdateGroup(model);
        
        if (!response.Success)
            return response.Error.Contains("does not exist")
                ? NotFound(new { Message = response.Error })
                : BadRequest(new { Message = response.Error });
                
        return Ok(response.Data);
    }
    
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGroup([FromRoute]int id)
    {
        var response = await groupService.DeleteGroup(id);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
            
        return NoContent();
    }
    
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpPost("add-user/{userId:int}/{groupId:int}")]
    public async Task<IActionResult> AddUserToGroup(int userId, int groupId)
    {
        var response = await groupService.AddUserToGroup(userId, groupId);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
            
        return Ok(response.Data);
    }
    
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpDelete("remove-user/{userId:int}/{groupId:int}")]
    public async Task<IActionResult> RemoveUserFromGroup(int userId, int groupId)
    {
        var response = await groupService.RemoveUserFromGroup(userId, groupId);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
            
        return Ok(response.Data);
        
    }
    
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpPut("set-leader/{userId:int}/{groupId:int}")]
    public async Task<IActionResult> SetLeaderOfGroup(int userId, int groupId)
    {
        var response = await groupService.SetLeaderOfGroup(userId, groupId);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
        return Ok(response.Data);
    }

    [Authorize(policy: "TeamLeaderOnly")]
    [HttpGet("image/{groupId:int}")]
    public async Task<IActionResult> GetGroupImage(int groupId)
    {
        var result = await groupService.GetGroupImagePath(groupId);
        if (!result.Success)
            return NotFound(new { Message = result.Error });
        
        //I'm not returning files because it's not any kind private data    
        return File(result.Data, "image/*");
    }
    
    [Authorize(policy: "TeamLeaderOnly")]
    [HttpPost("upload-image"),DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadGroupImage([FromForm]int groupId,IFormFile? image,[FromForm] bool isUpdate = false)
    {
        if (image == null || image.Length == 0)
            return BadRequest("Not send any image");
        
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Bad type of image");
        
        try
        {
            if (isUpdate)
            {
                var updatedPath = await groupService.SaveGroupImage(groupId,image, true);
                return updatedPath.Success ? Ok(new { Path = updatedPath.Data}) : BadRequest(new {Message = updatedPath.Error});
            }
            var imagePath = await groupService.SaveGroupImage(groupId, image);
            return imagePath.Success ? Ok(new {Path = imagePath.Data}) : BadRequest(new {Message = imagePath.Error});
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "An error occurred while uploading the image", Details = ex.Message });
        }
    }
}