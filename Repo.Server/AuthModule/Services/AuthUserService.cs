using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.auth;
using Repo.Server.Controllers.Interfaces;

namespace Repo.Server.Controllers;

public class AuthUserService : IAuthUserService
{
    private readonly MyDbContext _context;
    private readonly AuthenticationHelpers _authenticationHelpers;
    public AuthUserService(MyDbContext context, AuthenticationHelpers auth)
    {
        _context = context;
        _authenticationHelpers = auth;
    }

    public async Task<Response<User>> CreateUser(RegistrationModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            
            if (_context.Set<User>().Any(x => x.Email == model.Email))
            {
                return Response<User>.Fail("User with this email already exists");
            }

            if (_context.Set<User>().Any(x => x.Nickname == model.Nickname))
            {
                return Response<User>.Fail("User with this nickname already exists");
            }

            byte[] salt = AuthenticationHelpers.GenerateSalt(64);

            User user = new()
            {
                Login = model.Login,
                Nickname = model.Nickname,
                Email = model.Email,
                Password = AuthenticationHelpers.GeneratePasswordHash(model.Password, salt),
                Salt = salt,
                Name = model.Name,
                Surname = model.Surname
            };
  
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();
            user.Roles = new List<Role>();
            
            //Adding Roles to user
            if (model.Role != null && model.Role.Any())
            {
                foreach (var roleName in model.Role)
                {
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Role_Name == roleName);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }
                }
            }
            else
            {
                // Basic role if not find any "User"
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Role_Name == "User");
                if (defaultRole != null)
                {
                    user.Roles.Add(defaultRole);
                }
            }
            
            //Adding refresh token to database
            var token = new RefreshToken()
            {
                Token = _authenticationHelpers.GenerateRefreshToken(),
                User_ID = user.ID,
                ExpireDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now
            };
            
            await _context.SaveChangesAsync();
            _context.Set<RefreshToken>().Add(token);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Response<User>.Ok(user);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Response<User>.Fail($"Error during creating of user: {e.Message}");
        }
    }

    public async Task<Response<TokenModel>> Login(LoginModel model)
    {
        try
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(e => e.Nickname == model.Nickname);

            if (user == null)
            {
                return Response<TokenModel>.Fail("User with this nickname does not exist");
            }

            if (!AuthenticationHelpers.VerifyPasswordHash(model.Password,user.Password, user.Salt))
            {
                return Response<TokenModel>.Fail("Wrong password");
            }
            
            //Validate if our refresh token is valid, if not we make new one
            var refreshToken = await _context.Set<RefreshToken>().FirstOrDefaultAsync(e => e.User_ID == user.ID);
            var Roles = await GetUserRoles(user.ID);
            
            if (refreshToken != null && refreshToken.RevokedAt == null &&
                refreshToken.ExpireDate > DateTime.Now.AddDays(1))
            {
                return Response<TokenModel>.Ok(new TokenModel()
                {
                    AccessToken = _authenticationHelpers.GenerateToken(user.Nickname,Roles),
                    RefreshToken = refreshToken.Token
                });
            }
            //creatng new token if neccesary
            var tokens = _authenticationHelpers.GenerateTokens(user.Nickname,Roles);
            var token = new RefreshToken()
            {
                Token = tokens.RefreshToken,
                User_ID = user.ID,
                ExpireDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now
            };
                
            await _context.Set<RefreshToken>().AddAsync(token);
            await _context.SaveChangesAsync();
            
            return Response<TokenModel>.Ok(tokens);
            
        }catch (Exception e)
        {
            return Response<TokenModel>.Fail($"Error during login: {e.Message}");
        }
    }

    public async Task<Response<TokenModel>> RefreshToken(TokenModel tokenModel)
    {
       var accessToken = tokenModel.AccessToken;
       var refreshToken = tokenModel.RefreshToken;
       
       var principals = _authenticationHelpers.GetPrincipalFromExpiredToken(accessToken);
       
       var username = principals.Identity.Name;

       if (username == null)
       {
           return Response<TokenModel>.Fail("Invalid token, user was not found");
       }

       if (await ValidateRefreshToken(username, refreshToken) == false)
       {
           return Response<TokenModel>.Fail("Invalid refresh token");
       }
       
       var newToken = _authenticationHelpers.GenerateToken(username);

       return Response<TokenModel>.Ok(new TokenModel()
           {
               AccessToken = newToken,
               RefreshToken = refreshToken
           }
       );

    }
    
    private async Task<bool> ValidateRefreshToken(string username, string refreshToken)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(e => e.Nickname == username);
        
        if (user == null)
        {
            return false;
        }
    
        var token = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(e => 
                e.Token == refreshToken && 
                e.User_ID == user.ID && 
                e.RevokedAt == null && 
                e.ExpireDate > DateTime.Now);
            
        return token != null;
    }
    
    private async Task<List<string>> GetUserRoles(int userId)
    {
        return await _context.Users.Include(u=>u.Roles)
            .Where(u => u.ID == userId)
            .SelectMany(u => u.Roles)
            .Select(r => r.Role_Name)
            .ToListAsync();
    }

   
}