using Repo.Core.Models;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRoleByName(string name);
    Task<List<Role>> GetAllRoles();
}