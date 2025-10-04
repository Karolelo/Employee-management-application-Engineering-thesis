using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "UserOnly")]
public class CalendarController : ControllerBase 
{
    private readonly ICalendarService _calendarService;
    
    public CalendarController(ICalendarService calendarService)
    {
        this._calendarService = calendarService;
    }
    
    [HttpGet("events/user/{userId:int}")]
     public async Task<IActionResult> GetAllUserEvents(int userId)
     {
         var response = await _calendarService.GetAllUserEvents(userId);

         return response.Success 
             ? Ok(response.Data)
             : NotFound(new { Message = response.Error }); 
     }
}