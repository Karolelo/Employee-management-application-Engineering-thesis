using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }
    
    //[HttpGet] methods
    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = await _courseService.GetCourses(q, page, pageSize);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var response = await _courseService.GetCourseById(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }

    [HttpGet("{id:int}/participants")]
    public async Task<IActionResult> GetParticipants(int id)
    {
        var response = await _courseService.GetParticipants(id);
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { message = response.Error });
    }
    
    //[HttpPost] methods
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseMiniDTO dto)
    {
        var response = await _courseService.CreateCourse(dto);
        return response.Success
            ? CreatedAtAction(nameof(GetCourseById), new { id = response.Data!.ID }, response.Data)
            : BadRequest(new { message = response.Error });
    }
    
    [HttpPost("{id:int}/enroll")]
    public async Task<IActionResult> EnrollUser(int id)
    {
        if (!int.TryParse(User.FindFirst("id")?.Value, out var userId))
        {
            return Unauthorized(new { message = "Missing user id claim" });
        }
        
        var response = await _courseService.EnrollUser(id, userId);
        return response.Success
            ? NoContent()
            : BadRequest(new { message = response.Error });
    }
    
    //[HttpPut] methods
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseMiniDTO dto)
    {
        var response = await _courseService.UpdateCourse(id, dto);
        if (!response.Success)
            return response.Error.Equals("Course not found")
                ? NotFound(new { message = response.Error })
                : BadRequest(new { message = response.Error });
        return NoContent();
    }
    
    //[HttpDelete] methods
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var response = await _courseService.DeleteCourse(id);
        if (response.Success)
            return NoContent();
        
        return response.Error switch
        {
            "Course not found"      => NotFound(new { Message = response.Error }),
            var msg when msg?.StartsWith("Course has") == true
                => Conflict(new { message = response.Error }),
            _                       => BadRequest(new { Message = response.Error })
        };
    }
    
    [HttpDelete("{id:int}/enroll")]
    public async Task<IActionResult> UnenrollUser(int id)
    {
        if (!int.TryParse(User.FindFirst("id")?.Value, out var userId))
            return Unauthorized(new { message = "Missing user id claim" });
        
        var response = await _courseService.UnenrollUser(id, userId);
        if (!response.Success)
            return response.Error.Equals("Course not found")
                ? NotFound(new { message = response.Error })
                : BadRequest(new { message = response.Error });
        return NoContent();
    }
}