using Repo.Core.Models.api;
using Repo.Server.WorkTimeModule.DTOs;

namespace Repo.Server.WorkTimeModule.Interfaces;

public interface IWorkEntryService
{
    Task<Response<WorkEntryDTO>> GetEntryById(int id);
    Task<Response<ICollection<WorkEntryDTO>>> GetUserEntries(int userId, DateOnly? from, DateOnly? to);

    Task<Response<ICollection<WorkEntryDTO>>> GetEntriesForWorkTable(
        int workTableId,
        DateOnly? from,
        DateOnly? to);
    
    Task<Response<ICollection<WorkEntryDTO>>> GetEntriesForAdmin(
        int? userId,
        DateTime? from,
        DateTime? to);

    Task<Response<WorkEntryDTO>> CreateEntryForUser(int userId, WorkEntryCreateDTO dto);
    Task<Response<WorkEntryDTO>> UpdateEntry(int id, WorkEntryCreateDTO dto);
    Task<Response<object>> DeleteEntry(int id);

    Task<Response<WorkSummaryDTO>> GetUserMonthlySummary(int userId, int year, int month);
}