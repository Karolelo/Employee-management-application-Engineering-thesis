using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models;
using Repo.Core.Models.DTOs;
using Repo.Server.TaskModule.interafaces;

namespace Repo.Server.TaskModule;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PriorityController : ControllerBase
{
    private readonly IPriorityService _priorityService;
    
    public PriorityController(IPriorityService priorityService)
        {
        _priorityService = priorityService;
        }

    [HttpPost("add")]
    public async Task<IActionResult> AddPriority(PriorityDTO model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var response = await _priorityService.AddPriority(model);

        var priority = response.Data;

        return response.Success
            ? CreatedAtAction(nameof(GetPriorityById), new { id = priority.ID }, $"Added new priority with ID: {priority.ID}")
            : BadRequest(new { Message = response.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePriority(PriorityDTO model, int id)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
         var response = await _priorityService.UpdatePriority(model, id);
         
         if (!response.Success)
             return response.Error.Equals("Priority not found")
                 ? NotFound(new { Message = response.Error })
                 : BadRequest(new { Message = response.Error });
         
         return Ok(response.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPriorityById(int id)
    {
        throw new NotImplementedException();
    }
}