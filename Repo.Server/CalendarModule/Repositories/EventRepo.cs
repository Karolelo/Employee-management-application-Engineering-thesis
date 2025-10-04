using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.calendar;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Repositories;

public class EventRepo : IEventRepository
{
    private readonly MyDbContext _context;
    
    public EventRepo(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<UserEventsDisplayable>> GetAllUserEvents(int id)
    {
        var events = await _context.Database
            .SqlQuery<UserEventsDisplayable>($"Exec sp_getUserEvents @userID = {id}")
            .ToListAsync();
        return events;
    }

    public async Task<List<UserEventsDisplayable>> GetUserEventsFromDate(int id, DateTime date)
    {
        var result = await GetAllUserEvents(id);
        var events = result
            .Where(e=>e.Start.CompareTo(date) >= 0 && e.End.CompareTo(date) <= 0)
            .ToList();
        return events;
    }

    public Task<IEnumerable<Event>> GetUserEventsToDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<Event> AddGlobalEvent(Event @event)
    {
        throw new NotImplementedException();
    }

    public Task<Event> AddUserEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Task<Event> UpdateEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Task<Event> DeleteEvent(int id)
    {
        throw new NotImplementedException();
    }
}