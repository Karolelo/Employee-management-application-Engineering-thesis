using Repo.Core.Models;
using Repo.Core.Models.api;

namespace Repo.Server.CalendarModule.Interfaces;

public interface ICalendarService
{
    //Methods for getting events
    Response<IEnumerable<Event>> GetAllUserEvents(int id);
    Response<IEnumerable<Event>> GetUserEventsFromDate(int id,DateTime date);
    Response<IEnumerable<Event>> GetUserEventsToDate(int id,DateTime date);
    
    //Methods for posting evetns
    Response<Event> AddGlobalEvent(Event @event);
    Response<Event> AddUserEvent(Event @event, int id);
    
    //Methods for update
    Response<Event> UpdateEvent(Event @event, int id);
    
    //Methods for deleting
    Response<Event> DeleteEvent(int id);
}