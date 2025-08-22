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
    //Stary no przydała by się metoda zwracająca wszystkie priority i ewentuallnie szukająca tego priority po nazwie !
    private readonly IPriorityService _priorityService;
    private readonly ITaskService _taskService;
    
    public PriorityController(IPriorityService priorityService, ITaskService taskService)
    {
        _priorityService = priorityService;
        _taskService = taskService;
    }
    
    //Methods for getting priority
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPriorityById(int id)
    {
        var response = await _priorityService.GetPriorityById(id);
        
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<IActionResult> GetTasksForPriority(int id)
    {
        var response = await _taskService.GetTasksByPriorityId(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    //Methods for creating priority
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

    //Methods for updating priority
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
}