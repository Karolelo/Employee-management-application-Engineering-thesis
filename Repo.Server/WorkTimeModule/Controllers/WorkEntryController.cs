using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Server.WorkTimeModule.DTOs;
using Repo.Server.WorkTimeModule.Interfaces;

namespace Repo.Server.WorkTimeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkEntryController : ControllerBase
{
    private readonly IWorkEntryService _service;

    public WorkEntryController(IWorkEntryService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEntryById(int id)
    {
        var response = await _service.GetEntryById(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }
    
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetEntriesForAdmin(
        [FromQuery] int? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var response = await _service.GetEntriesForAdmin(userId, from, to);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserEntries(
        int userId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var response = await _service.GetUserEntries(userId, from, to);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("user/{userId:int}/summary")]
    public async Task<IActionResult> GetUserMonthlySummary(
        int userId,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var response = await _service.GetUserMonthlySummary(userId, year, month);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpPost("user/{userId:int}")]
    public async Task<IActionResult> CreateEntryForUser(int userId, [FromBody] WorkEntryCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _service.CreateEntryForUser(userId, dto);
        return response.Success
            ? CreatedAtAction(nameof(GetEntryById), new { id = response.Data!.ID }, response.Data)
            : BadRequest(new { message = response.Error });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEntry(int id, [FromBody] WorkEntryCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await _service.UpdateEntry(id, dto);
        if (!response.Success)
            return response.Error switch
            {
                "Work entry not found" => NotFound(new { message = response.Error }),
                _                      => BadRequest(new { message = response.Error })
            };

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEntry(int id)
    {
        var response = await _service.DeleteEntry(id);
        return response.Success
            ? NoContent()
            : NotFound(new { message = response.Error });
    }
}
