using Repo.Core.Models;

namespace Repo.Server.UserManagmentModule.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetAllRoles();
}