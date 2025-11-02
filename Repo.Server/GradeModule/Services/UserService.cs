using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models.api;
using Repo.Server.GradeModule.DTOs;
using Repo.Server.GradeModule.Interfaces;

namespace Repo.Server.GradeModule.Services;

public class UserService : IUserService
{
    private readonly MyDbContext _context;

    public UserService(MyDbContext context)
    {
        _context = context;
    }
    
    public async Task<Response<ICollection<UserMiniDTO>>> GetUsers(string? q, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 200) pageSize = 200;
        
        var query = _context.Users.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            bool qIsId = int.TryParse(q, out var idFilter);
            query = query.Where(user =>
                user.Login.Contains(q) ||
                (user.Nickname != null && user.Nickname.Contains(q)) ||
                (qIsId && user.ID == idFilter)
            );
        }
        
        var data = await query
            .OrderBy(user => user.Nickname ?? user.Login)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UserMiniDTO
            {
                ID = user.ID,
                Login = user.Login,
                Nickname = user.Nickname
            })
            .ToListAsync();

        return data.Count == 0
            ? Response<ICollection<UserMiniDTO>>.Fail("No users found")
            : Response<ICollection<UserMiniDTO>>.Ok(data);
    }

    public async Task<Response<UserMiniDTO>> GetUserById(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.ID == id)
            .Select(u => new UserMiniDTO
            {
                ID = u.ID,
                Login = u.Login,
                Nickname = u.Nickname
            })
            .FirstOrDefaultAsync();

        return user == null
            ? Response<UserMiniDTO>.Fail("User not found")
            : Response<UserMiniDTO>.Ok(user);
    }
}