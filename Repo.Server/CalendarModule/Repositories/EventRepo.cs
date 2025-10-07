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
            .Where(e=>e.Start.CompareTo(date) >= 0)
            .ToList();
        return events;
    }

    public async Task<List<UserEventsDisplayable>> GetUserEventsToDate(int id, DateTime date)
    {
        var result = await GetAllUserEvents(id);
        var events = result
            .Where(e=>e.End <= date)
            .ToList();
        return events;
    }

    public async Task<List<UserEventsDisplayable>> GetUserEventsFromTo(int id, DateTime from, DateTime to)
    {
        var result = await GetAllUserEvents(id);
        var events = result
            .Where(e=>e.Start.CompareTo(from) >= 0 && e.End <= to)
            .ToList();
        return events;
    }

    public async Task<bool> ChangeEventColor(int eventId, string color)
    {
        try
        {
         var eventToUpdate = await _context.Events.FirstOrDefaultAsync(e => e.ID == eventId);
         if (eventToUpdate == null)
         {
             return false;
         }
         
         eventToUpdate.BackColor = color;
         await _context.SaveChangesAsync();
         return true;
        }catch (Exception e)
        {
            return false;
        }
    }
    
}