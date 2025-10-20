using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.user;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(policy: "TeamLeaderOnly")]
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
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById([FromRoute]int id)
    {
        var response = await groupService.GetGroupById(id);
        
        if (!response.Success)
            return NotFound(new { Message = response.Error });
            
        return Ok(response.Data);
    }

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
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup([FromRoute]int id)
    {
        var response = await groupService.DeleteGroup(id);
        
        if (!response.Success)
            return BadRequest(new { Message = response.Error });
            
        return NoContent();
    }
}