using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradeController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }
    
    //[HttpGet] methods
    [HttpGet]
    public async Task<IActionResult> GetGrades([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = await _gradeService.GetGrades(q, page, pageSize);
        return response.Success ? Ok(response.Data) : NotFound(new { message = response.Error });
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGradeById(int id)
    {
        var response = await _gradeService.GetGradeById(id);
        return response.Success ? Ok(response.Data) : NotFound(new { message = response.Error });
    }
    
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserGrades(int userId)
    {
        var response = await _gradeService.GetUserGrades(userId);
        return response.Success ? Ok(response.Data) : NotFound(new { message = response.Error });
    }
    
    //[HttpPost] methods
    [HttpPost("user/{userId:int}")]
    public async Task<IActionResult> CreateGrade(int userId, [FromBody] GradeMiniDTO dto)
    {
        var response = await _gradeService.CreateGrade(userId, dto);
        return response.Success
            ? CreatedAtAction(nameof(GetGradeById), new { id = response.Data!.ID }, response.Data)
            : BadRequest(new { message = response.Error });
    }
    
    //[HttpPut] methods
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] GradeMiniDTO dto)
    {
        var response = await _gradeService.UpdateGrade(id, dto);
        if (!response.Success)
            return response.Error switch
            {
                "Grade not found" => NotFound(new { message = response.Error }),
                _                 => BadRequest(new { message = response.Error })
            };

        return NoContent();
    }
    
    //[HttpDelete] methods
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var response = await _gradeService.DeleteGrade(id);
        return response.Success ? NoContent() : NotFound(new { message = response.Error });
    }
}