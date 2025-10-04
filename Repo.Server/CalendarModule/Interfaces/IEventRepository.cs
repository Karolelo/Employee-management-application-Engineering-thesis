using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.calendar;

namespace Repo.Server.CalendarModule.Interfaces;

public interface IEventRepository
{
    Task<List<UserEventsDisplayable>> GetAllUserEvents(int id);
    Task<List<Event>> GetUserEventsFromDate(int id, DateTime date);
    Task<List<Event>> GetUserEventsToDate(int id, DateTime date);
    
    Task<Event> AddGlobalEvent(Event @event);
    Task<Event> AddUserEvent(Event @event, int id);
    
    Task<Event> UpdateEvent(Event @event, int id);
    
    Task<Event> DeleteEvent(int id);
}