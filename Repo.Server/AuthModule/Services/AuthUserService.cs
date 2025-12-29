
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.UnityOfWork;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.auth;
using Repo.Server.AuthModule.Interfaces;
using Repo.Server.Controllers.Interfaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Task = System.Threading.Tasks.Task;

public class AuthUserService : IAuthUserService
{
    private readonly AuthenticationHelpers _authenticationHelpers;
    private readonly IUnityOfWork<MyDbContext> _unityOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    private const int RefreshTokenExpirationDays = 7;
    private const string DefaultRoleName = "User";

    public AuthUserService(
        AuthenticationHelpers auth,
        IUnityOfWork<MyDbContext> unityOfWork,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _authenticationHelpers = auth;
        _unityOfWork = unityOfWork;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Response<User>> CreateUser(RegistrationModel model)
    {
        _unityOfWork.CreateTransaction();
        try
        {
            var existingUserByEmail = await _userRepository.GetUserByEmail(model.Email);
            if (existingUserByEmail is not null)
            {
                return Response<User>.Fail("User with this email already exists");
            }

            var existingUserByNickname = await _userRepository.GetUserByNickname(model.Nickname);
            if (existingUserByNickname is not null)
            {
                return Response<User>.Fail("User with this nickname already exists");
            }

            var user = await CreateUserEntity(model);
            await AssignRolesToUser(user, model.Role);
            
            var refreshToken = CreateRefreshToken(user.ID);
            await _refreshTokenRepository.Add(refreshToken);
            
            _unityOfWork.Save();
            _unityOfWork.Commit();
            
            return Response<User>.Ok(user);
        }
        catch (Exception e)
        {
            _unityOfWork.Rollback();
            return Response<User>.Fail($"Error during creating of user: {e.Message}");
        }
    }

    public async Task<Response<TokenModel>> Login(LoginModel model)
    {
        try
        {
            var user = await _userRepository.GetUserByLogin(model.Login);
            if (user == null)
            {
                return Response<TokenModel>.Fail("User with this login does not exist");
            }

            if (!AuthenticationHelpers.VerifyPasswordHash(model.Password, user.Password, user.Salt))
            {
                return Response<TokenModel>.Fail("Wrong password");
            }

            var roles = await _userRepository.GetUserRoles(user.ID);
            var refreshToken = await _refreshTokenRepository.FindByUserId(user.ID);
            
            // Check if existing refresh token is valid
            if (refreshToken != null && await ValidateRefreshToken(user.Nickname, refreshToken.Token))
            {
                return Response<TokenModel>.Ok(new TokenModel
                {
                    AccessToken = _authenticationHelpers.GenerateToken(user.ID, user.Nickname, roles),
                    RefreshToken = refreshToken.Token
                });
            }

            // Revoke old token and create new one
            if (refreshToken != null)
            {
                refreshToken.RevokedAt = DateTime.Now;
                await _refreshTokenRepository.Update(refreshToken);
            }

            var newRefreshToken = CreateRefreshToken(user.ID);
            await _refreshTokenRepository.Add(newRefreshToken);
            
            return Response<TokenModel>.Ok(new TokenModel
            {
                AccessToken = _authenticationHelpers.GenerateToken(user.ID, user.Nickname, roles),
                RefreshToken = newRefreshToken.Token
            });
        }
        catch (Exception e)
        {
            return Response<TokenModel>.Fail($"Error during login: {e.Message}");
        }
    }

    public async Task<Response<TokenModel>> RefreshToken(TokenModel tokenModel)
    {
        var principals = _authenticationHelpers.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
        if (principals == null)
        {
            return Response<TokenModel>.Fail("Bad token format");
        }

        var idClaim = principals.FindFirst("id");
        if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
        {
            return Response<TokenModel>.Fail("Invalid user ID in token");
        }

        var username = principals.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Response<TokenModel>.Fail("Bad token: no user was found");
        }

        if (!await ValidateRefreshToken(username, tokenModel.RefreshToken))
        {
            return Response<TokenModel>.Fail("Bad refresh token");
        }

        var roles = await _userRepository.GetUserRoles(userId);
        var newAccessToken = _authenticationHelpers.GenerateToken(userId, username, roles);

        return Response<TokenModel>.Ok(new TokenModel
        {
            AccessToken = newAccessToken,
            RefreshToken = tokenModel.RefreshToken
        });
    }

    private async Task<User> CreateUserEntity(RegistrationModel model)
    {
        var salt = AuthenticationHelpers.GenerateSalt(64);
        
        var user = new User
        {
            Login = model.Login,
            Nickname = model.Nickname,
            Email = model.Email,
            Password = AuthenticationHelpers.GeneratePasswordHash(model.Password, salt),
            Salt = salt,
            Name = model.Name,
            Surname = model.Surname,
            Roles = new List<Role>()
        };

        await _userRepository.CreateUser(user);
        return user;
    }

    private async Task AssignRolesToUser(User user, List<string>? requestedRoles)
    {
        var roleNames = requestedRoles is { Count: > 0 } 
            ? requestedRoles 
            : new List<string> { DefaultRoleName };
        
        var existingRoles = await _roleRepository.GetAllRoles();
        user.Roles = existingRoles
            .Where(r => roleNames.Contains(r.Role_Name))
            .ToList();

        await _userRepository.UpdateUser(user);
    }

    private async Task<bool> ValidateRefreshToken(string nickname, string token)
    {
        var user = await _userRepository.GetUserByNickname(nickname);
        if (user is null)
        {
            return false;
        }

        var tokenToCheck = await _refreshTokenRepository.FindByUserId(user.ID);
        if (tokenToCheck is null)
        {
            return false;
        }
        
        return tokenToCheck.CreatedAt <= DateTime.Now
               && tokenToCheck.ExpireDate > DateTime.Now
               && tokenToCheck.Token == token
               && tokenToCheck.User_ID == user.ID
               && tokenToCheck.RevokedAt is null;
    }

    private RefreshToken CreateRefreshToken(int userId)
    {
        return new RefreshToken
        {
            Token = _authenticationHelpers.GenerateRefreshToken(),
            User_ID = userId,
            ExpireDate = DateTime.Now.AddDays(RefreshTokenExpirationDays),
            CreatedAt = DateTime.Now
        };
    }
}