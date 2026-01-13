using Repo.Core.Models;

namespace Repo.Server.TaskModule.interafaces;

public interface IPriorityRepository
{
    Task<Priority?> GetPriorityByName(string name);
}