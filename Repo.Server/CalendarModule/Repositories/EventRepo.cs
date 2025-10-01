using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Repositories;

public class EventRepo : IEventRepository
{
    private readonly MyDbContext _context;
    
    public EventRepo(MyDbContext context)
    {
        _context = context;
    }
    
    public Response<IEnumerable<Event>> GetAllUserEvents(int id)
    {
        _context.Events
            .AsNoTracking()
            .Where(e=> e.Course. == id)
            .ToList();
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