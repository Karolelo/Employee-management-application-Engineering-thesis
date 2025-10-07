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

    public async Task<Response<List<UserEventsDisplayable>>> GetUserEventsFromDate(int id, DateTime date)
    {
        try
        {
            var events = await _eventRepository.GetUserEventsFromDate(id, date);
            return Response<List<UserEventsDisplayable>>.Ok(events);
        }
        catch (SqlException e) when (e.Number == 50001)
        {
            return Response<List<UserEventsDisplayable>>.Fail(
                "User not found");
        }
    }

    public async Task<Response<List<UserEventsDisplayable>>> GetUserEventsToDate(int id, DateTime date)
    {
        try
        {
            var events = await _eventRepository.GetUserEventsToDate(id, date);
            return Response<List<UserEventsDisplayable>>.Ok(events);
        }
        catch (SqlException e) when (e.Number == 50001)
        {
            return Response<List<UserEventsDisplayable>>.Fail(
                "User not found");
        }
    }

    public async Task<Response<List<UserEventsDisplayable>>> GetUserEventsFromTo(int id, DateTime from, DateTime to)
    {
        try
        {
            var events = await _eventRepository.GetUserEventsFromTo(id, from,to);
            return Response<List<UserEventsDisplayable>>.Ok(events);
        }
        catch (SqlException e) when (e.Number == 50001)
        {
            return Response<List<UserEventsDisplayable>>.Fail(
                "User not found");
        }
    }

    
}