using Repo.Core.Models.profile;
using Repo.Core.Models.api;
namespace Repo.Server.ProfileModule;

public interface IUserProfileService
{
    Task<Response<UserProfileDTO>> GetProfileAsync(int userId);
    Task<Response<object>> ChangeEmailAsync(int userId, ChangeEmailDTO model);
    Task<Response<object>> ChangePasswordAsync(int userId, ChangePasswordDTO model);
}
