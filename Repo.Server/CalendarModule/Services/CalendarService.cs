using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Services;

public class CalendarService : ICalendarService
{
    public Response<IEnumerable<Event>> GetAllUserEvents(int id)
    {
        throw new NotImplementedException();
    }

    public Response<IEnumerable<Event>> GetUserEventsFromDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Response<IEnumerable<Event>> GetUserEventsToDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Response<Event> AddGlobalEvent(Event @event)
    {
        throw new NotImplementedException();
    }

    public Response<Event> AddUserEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Response<Event> UpdateEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Response<Event> DeleteEvent(int id)
    {
        throw new NotImplementedException();
    }
}