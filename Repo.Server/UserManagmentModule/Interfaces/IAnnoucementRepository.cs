using Repo.Core.Models;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IAnnoucementRepository
{
    Task<Announcement?> GetAnnoucementById(int annoucementId);
    Task<List<Announcement>> GetAllAnnouncements(int groupId);
    Task<Announcement> AddAnnouncement(Announcement announcement);
    Task<Announcement> UpdateAnnouncement(Announcement announcement);
    Task<bool> DeleteAnnouncement(int id);
}