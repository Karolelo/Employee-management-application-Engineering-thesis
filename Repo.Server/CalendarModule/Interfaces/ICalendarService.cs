using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.calendar;

namespace Repo.Server.CalendarModule.Interfaces;

public interface ICalendarService
{
    //Methods for getting events
    Task<Response<List<UserEventsDisplayable>>> GetAllUserEvents(int id);
    Task<Response<List<Event>>> GetUserEventsFromDate(int id, DateTime date);
    Task<Response<List<Event>>> GetUserEventsToDate(int id, DateTime date);
    
    //Methods for posting events
    Task<Response<Event>> AddGlobalEvent(Event @event);
    Task<Response<Event>> AddUserEvent(Event @event, int id);
    
    //Methods for update
    Task<Response<Event>> UpdateEvent(Event @event, int id);
    
    //Methods for deleting
    Task<Response<Event>> DeleteEvent(int id);
}