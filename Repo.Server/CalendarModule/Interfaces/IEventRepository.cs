using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.calendar;

namespace Repo.Server.CalendarModule.Interfaces;

public interface IEventRepository
{
    Task<List<UserEventsDisplayable>> GetAllUserEvents(int id);
    Task<List<UserEventsDisplayable>> GetUserEventsFromDate(int id, DateTime date);
    Task<List<UserEventsDisplayable>> GetUserEventsToDate(int id, DateTime date);
    Task<List<UserEventsDisplayable>> GetUserEventsFromTo(int id,DateTime from, DateTime to);
    //We were thinking about implementing this
    //but the calendar should not be modifed directly but through adding task etc.
    /*Task<Event> AddGlobalEvent(Event @event);
    Task<Event> AddUserEvent(Event @event, int userId);
    Task<Event> UpdateEvent(Event @event, int id);
    Task<Event> DeleteEvent(int id);*/
}