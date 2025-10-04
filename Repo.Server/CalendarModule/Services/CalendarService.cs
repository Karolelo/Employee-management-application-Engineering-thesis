using Microsoft.Data.SqlClient;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.calendar;
using Repo.Server.CalendarModule.Interfaces;

namespace Repo.Server.CalendarModule.Services;
//TODO zrobić może repo z userem i tam metody inne metody do walidacji, np sprawdzenie
//istnienia user w GetAllUser, teraz to zrobie przez try catcha, bo queryy rzuca bład 
public class CalendarService : ICalendarService
{
    private readonly IEventRepository _eventRepository;
    
    public CalendarService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    public async Task<Response<List<UserEventsDisplayable>>> GetAllUserEvents(int id)
    {
        try
        {
            var events = await _eventRepository.GetAllUserEvents(id);
            return Response<List<UserEventsDisplayable>>.Ok(events);
        }
        catch (SqlException e) when (e.Number == 50001)
        {
            return Response<List<UserEventsDisplayable>>.Fail(
                "User not found");
        }
        
    }

    public Response<IEnumerable<Event>> GetUserEventsFromDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    Task<Response<List<Event>>> ICalendarService.GetUserEventsFromDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<Response<List<Event>>> GetUserEventsToDate(int id, DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Event>> AddGlobalEvent(Event @event)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Event>> AddUserEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Event>> UpdateEvent(Event @event, int id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Event>> DeleteEvent(int id)
    {
        throw new NotImplementedException();
    }
}