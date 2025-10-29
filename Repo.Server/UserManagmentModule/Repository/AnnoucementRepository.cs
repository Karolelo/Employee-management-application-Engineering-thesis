using Microsoft.EntityFrameworkCore;

using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Repository;

public class AnnoucementRepository : IAnnoucementRepository
{
    private readonly MyDbContext _context;

    public AnnoucementRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Announcement?> GetAnnoucementById(int annoucementId)
    {
        return await _context.Announcements.FirstOrDefaultAsync(a=>a.ID==annoucementId);
    }

    public async Task<List<Announcement>> GetAllAnnouncements(int groupId)
    {
        return await _context.Announcements.Where(a=> a.Group_ID == groupId).ToListAsync();
    }

    public async Task<Announcement> AddAnnouncement(Announcement announcement)
    {
        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }

    public async Task<Announcement> UpdateAnnouncement(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }

    public async Task<bool> DeleteAnnouncement(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        _context.Announcements.Remove(announcement);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}