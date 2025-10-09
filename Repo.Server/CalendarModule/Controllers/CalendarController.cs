using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repo.Core.Models.calendar;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Controllers;

[ApiController]
[Route("api/[controller]/events/user/{userId:int}")]
[Authorize(Policy = "UserOnly")]
public class CalendarController : ControllerBase 
{
    private readonly ICalendarService _calendarService;
    
    public CalendarController(ICalendarService calendarService)
    {
        this._calendarService = calendarService;
    }
    
    [HttpGet]
     public async Task<IActionResult> GetAllUserEvents(int userId)
     {
         var response = await _calendarService.GetAllUserEvents(userId);

         return response.Success 
             ? Ok(response.Data)
             : NotFound(new { Message = response.Error }); 
     }

    [HttpGet("from")]
    public async Task<IActionResult> GetUserEventsFromDate(int userId,[FromQuery] DateTime date)
    {
        var response = await _calendarService.GetUserEventsFromDate(userId, date);
        
        return response.Success 
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }
    
    [HttpGet("to")]
    public async Task<IActionResult> GetUserEventsToDate(int userId,[FromQuery] DateTime date)
    {
        var response = await _calendarService.GetUserEventsToDate(userId, date);
        
        return response.Success 
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }
    
    [HttpGet("from-to")]
    public async Task<IActionResult> GetUserEventsFromToDates(int userId, [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var response = await _calendarService.GetUserEventsFromTo(userId, fromDate, toDate);
        
        return response.Success
            ? Ok(response.Data)
            : NotFound(new { Message = response.Error });
    }

    [HttpPut("/api/Calendar/events/{eventId:int}/color")]
    public async Task<IActionResult> UpdateEventColor(int eventId, [FromBody] ColorUpdateDto color)
    {
        var response = await _calendarService.ChangeEventColor(eventId, color.Color);
        
        return response.Success
            ? Ok("Data has been change")
            : NotFound(new { Message = response.Error });
    }
    
}