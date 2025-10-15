using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Server.UserManagmentModule.Interfaces;

namespace Repo.Server.UserManagmentModule.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly MyDbContext _context;
    
    public RoleRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllRoles()
    {
        return await _context.Roles.ToListAsync();
    }
}