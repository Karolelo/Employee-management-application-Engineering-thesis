using Repo.Core.Models;

namespace Repo.Server.TaskModule.interafaces;

public interface IStatusRepository
{
    Task<Status?> GetStatusByName(string name);
}