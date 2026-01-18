using Repo.Core.Models;
using Task = System.Threading.Tasks.Task;
namespace Repo.Server.AuthModule.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> FindByUserId(int userId);
    Task<RefreshToken> Add(RefreshToken refreshToken);
    Task<RefreshToken> Update(RefreshToken refreshToken);
    Task Delete(int tokenId);
}