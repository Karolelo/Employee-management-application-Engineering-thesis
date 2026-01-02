using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Server.WorkTimeModule.DTOs;
using Repo.Server.WorkTimeModule.Interfaces;

namespace Repo.Server.WorkTimeModule.Services;

public class WorkEntryService : IWorkEntryService
{
    private readonly MyDbContext _context;

    public WorkEntryService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Response<WorkEntryDTO>> GetEntryById(int id)
    {
        var entry = await _context.WorkEntries
            .AsNoTracking()
            .Where(e => e.ID == id)
            .Select(e => new WorkEntryDTO
            {
                ID = e.ID,
                WorkTable_ID = e.WorkTable_ID,
                Task_ID = e.Task_ID,
                Work_Date = DateOnly.FromDateTime(e.Work_Date),
                Hours_Worked = e.Hours_Worked,
                Comment = e.Comment,
                TaskName = e.Task != null ? e.Task.Name : null,
                UserNickname = e.WorkTable.User.Nickname
            })
            .FirstOrDefaultAsync();

        return entry == null
            ? Response<WorkEntryDTO>.Fail("Work entry not found")
            : Response<WorkEntryDTO>.Ok(entry);
    }
    
    public async Task<Response<ICollection<WorkEntryDTO>>> GetEntriesForAdmin(
        int? userId, DateTime? from, DateTime? to)
    {
        var query = _context.WorkEntries
            .AsNoTracking()
            .Include(e => e.WorkTable)
            .ThenInclude(wt => wt.User)
            .Include(e => e.Task)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(e => e.WorkTable.User_ID == userId.Value);

        if (from.HasValue)
            query = query.Where(e => e.Work_Date >= from.Value.Date);
        if (to.HasValue)
            query = query.Where(e => e.Work_Date <= to.Value.Date);

        var data = await query
            .OrderByDescending(e => e.Work_Date)
            .Select(e => new WorkEntryDTO
            {
                ID = e.ID,
                Work_Date = DateOnly.FromDateTime(e.Work_Date),
                Hours_Worked = e.Hours_Worked,
                Comment = e.Comment,
                Task_ID = e.Task_ID,
                TaskName = e.Task != null ? e.Task.Name : null,
                UserID = e.WorkTable.User_ID,
                UserNickname = e.WorkTable.User.Nickname
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<WorkEntryDTO>>.Fail("No work entries found")
            : Response<ICollection<WorkEntryDTO>>.Ok(data);
    }

    public async Task<Response<ICollection<WorkEntryDTO>>> GetUserEntries(
        int userId,
        DateOnly? from,
        DateOnly? to)
    {
        var hasUser = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.ID == userId);

        if (!hasUser)
            return Response<ICollection<WorkEntryDTO>>.Fail("User not found");

        var query = _context.WorkEntries
            .AsNoTracking()
            .Include(e => e.WorkTable)
            .ThenInclude(wt => wt.User)
            .Include(e => e.Task)
            .Where(e => e.WorkTable.User_ID == userId)
            .AsQueryable();

        if (from.HasValue)
        {
            var fromDateTime = from.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(e => e.Work_Date >= fromDateTime);
        }

        if (to.HasValue)
        {
            var toDateTime = to.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(e => e.Work_Date <= toDateTime);
        }

        var list = await query
            .OrderByDescending(e => e.Work_Date)
            .Select(e => new WorkEntryDTO
            {
                ID = e.ID,
                WorkTable_ID = e.WorkTable_ID,
                Task_ID = e.Task_ID,
                Work_Date = DateOnly.FromDateTime(e.Work_Date),
                Hours_Worked = e.Hours_Worked,
                Comment = e.Comment,
                TaskName = e.Task != null ? e.Task.Name : null,
                UserNickname = e.WorkTable.User.Nickname
            })
            .ToListAsync();

        return list.Count == 0
            ? Response<ICollection<WorkEntryDTO>>.Fail("No work entries found")
            : Response<ICollection<WorkEntryDTO>>.Ok(list);
    }

    public async Task<Response<ICollection<WorkEntryDTO>>> GetEntriesForWorkTable(
        int workTableId,
        DateOnly? from,
        DateOnly? to)
    {
        var query = _context.WorkEntries
            .AsNoTracking()
            .Where(e => e.WorkTable_ID == workTableId);

        if (from.HasValue)
        {
            var fromDate = from.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(e => e.Work_Date >= fromDate);
        }

        if (to.HasValue)
        {
            var toDate = to.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(e => e.Work_Date <= toDate);
        }

        var list = await query
            .OrderBy(e => e.Work_Date)
            .Select(e => new WorkEntryDTO
            {
                ID = e.ID,
                WorkTable_ID = e.WorkTable_ID,
                Task_ID = e.Task_ID,
                Work_Date = DateOnly.FromDateTime(e.Work_Date),
                Hours_Worked = e.Hours_Worked,
                Comment = e.Comment,
                TaskName = e.Task != null ? e.Task.Name : null,
                UserNickname = e.WorkTable.User.Nickname
            })
            .ToListAsync();

        return list.Count == 0
            ? Response<ICollection<WorkEntryDTO>>.Fail("No work entries found")
            : Response<ICollection<WorkEntryDTO>>.Ok(list);
    }

    public async Task<Response<WorkEntryDTO>> CreateEntryForUser(int userId, WorkEntryCreateDTO dto)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.WorkTables)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
                return Response<WorkEntryDTO>.Fail("User not found");

            var workTable = await _context.WorkTables
                .FirstOrDefaultAsync(wt => wt.User_ID == userId);

            if (workTable == null)
            {
                workTable = new WorkTable
                {
                    User_ID = userId,
                    Hourly_Rate = 0m,
                    Account_Number = string.Empty
                };
                _context.WorkTables.Add(workTable);
                await _context.SaveChangesAsync();
            }

            Repo.Core.Models.Task? task = null;
            if (dto.Task_ID.HasValue)
            {
                task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.ID == dto.Task_ID.Value && t.Deleted == 0);
                if (task == null)
                    return Response<WorkEntryDTO>.Fail("Task not found");
            }

            var entry = new WorkEntry
            {
                WorkTable_ID = workTable.ID,
                Task_ID = dto.Task_ID,
                Work_Date = dto.Work_Date.ToDateTime(TimeOnly.MinValue),
                Hours_Worked = dto.Hours_Worked,
                Comment = dto.Comment?.Trim()
            };

            _context.WorkEntries.Add(entry);
            await _context.SaveChangesAsync();

            var result = new WorkEntryDTO
            {
                ID = entry.ID,
                WorkTable_ID = entry.WorkTable_ID,
                Task_ID = entry.Task_ID,
                Work_Date = DateOnly.FromDateTime(entry.Work_Date),
                Hours_Worked = entry.Hours_Worked,
                Comment = entry.Comment,
                TaskName = task?.Name,
                UserNickname = user.Nickname
            };

            return Response<WorkEntryDTO>.Ok(result);
        }
        catch (Exception e)
        {
            return Response<WorkEntryDTO>.Fail($"Error during creating work entry: {e.Message}");
        }
    }

    public async Task<Response<WorkEntryDTO>> UpdateEntry(int id, WorkEntryCreateDTO dto)
    {
        try
        {
            var entry = await _context.WorkEntries
                .Include(e => e.WorkTable)
                .FirstOrDefaultAsync(e => e.ID == id);

            if (entry == null)
                return Response<WorkEntryDTO>.Fail("Work entry not found");

            var workDate = dto.Work_Date.ToDateTime(TimeOnly.MinValue);

            var hasAbsence = await _context.AbsenceDays
                .AsNoTracking()
                .AnyAsync(a =>
                    a.WorkTable_ID == entry.WorkTable_ID &&
                    a.Start_Date <= dto.Work_Date &&
                    a.Finish_Date >= dto.Work_Date);

            if (hasAbsence)
                return Response<WorkEntryDTO>.Fail("Cannot register work time on absence day");

            if (dto.Task_ID.HasValue)
            {
                var taskExists = await _context.Tasks
                    .AsNoTracking()
                    .AnyAsync(t => t.ID == dto.Task_ID.Value && t.Deleted == 0);

                if (!taskExists)
                    return Response<WorkEntryDTO>.Fail("Task not found");
            }

            var existingHours = await _context.WorkEntries
                .AsNoTracking()
                .Where(e => e.WorkTable_ID == entry.WorkTable_ID
                            && e.Work_Date == workDate
                            && e.ID != id)
                .SumAsync(e => (decimal?)e.Hours_Worked) ?? 0m;

            if (existingHours + dto.Hours_Worked > 24)
                return Response<WorkEntryDTO>.Fail("Total hours for a single day cannot exceed 24");

            entry.Task_ID = dto.Task_ID;
            entry.Work_Date = workDate;
            entry.Hours_Worked = dto.Hours_Worked;
            entry.Comment = dto.Comment?.Trim();

            await _context.SaveChangesAsync();

            var result = new WorkEntryDTO
            {
                ID = entry.ID,
                WorkTable_ID = entry.WorkTable_ID,
                Task_ID = entry.Task_ID,
                Work_Date = dto.Work_Date,
                Hours_Worked = entry.Hours_Worked,
                Comment = entry.Comment
            };

            return Response<WorkEntryDTO>.Ok(result);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Response<WorkEntryDTO>.Fail("Concurrency conflict");
        }
        catch (Exception e)
        {
            return Response<WorkEntryDTO>.Fail($"Error during updating work entry: {e.Message}");
        }
    }

    public async Task<Response<object>> DeleteEntry(int id)
    {
        try
        {
            var entry = await _context.WorkEntries.FirstOrDefaultAsync(e => e.ID == id);
            if (entry == null)
                return Response<object>.Fail("Work entry not found");

            _context.WorkEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return Response<object>.Ok(new { removed = true });
        }
        catch (Exception e)
        {
            return Response<object>.Fail($"Error during deleting work entry: {e.Message}");
        }
    }

    public async Task<Response<WorkSummaryDTO>> GetUserMonthlySummary(int userId, int year, int month)
    {
        var hasUser = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.ID == userId);
        if (!hasUser)
            return Response<WorkSummaryDTO>.Fail("User not found");

        var workTable = await _context.WorkTables
            .AsNoTracking()
            .FirstOrDefaultAsync(wt => wt.User_ID == userId);
        if (workTable == null)
            return Response<WorkSummaryDTO>.Fail("WorkTable not found");

        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);

        var totalHours = await _context.WorkEntries
            .AsNoTracking()
            .Where(e => e.WorkTable_ID == workTable.ID
                        && e.Work_Date >= from
                        && e.Work_Date < to)
            .SumAsync(e => (decimal?)e.Hours_Worked) ?? 0m;

        var dto = new WorkSummaryDTO
        {
            User_ID = userId,
            Year = year,
            Month = month,
            TotalHours = totalHours,
            HourlyRate = workTable.Hourly_Rate,
            TotalAmount = totalHours * workTable.Hourly_Rate
        };

        return Response<WorkSummaryDTO>.Ok(dto);
    }
}
