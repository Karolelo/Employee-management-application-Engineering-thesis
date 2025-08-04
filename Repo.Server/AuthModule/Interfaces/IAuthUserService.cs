using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.auth;

namespace Repo.Server.Controllers.Interfaces;

public interface IAuthUserService
{
    Task<Response<User>> CreateUser(RegistrationModel model);
    Task<Response<string>> Login(LoginModel model);
    
    //TODO dodać wiecej metod
}