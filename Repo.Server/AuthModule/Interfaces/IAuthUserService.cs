using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.auth;

namespace Repo.Server.Controllers.Interfaces;

public interface IAuthUserService
{
    Task<Response<User>> CreateUser(RegistrationModel model);
    Task<Response<TokenModel>> Login(LoginModel model);
    Task<Response<TokenModel>> RefreshToken(TokenModel tokenModel);

}