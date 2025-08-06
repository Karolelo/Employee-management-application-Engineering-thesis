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
            return Response<User>.Ok(user);
        }
        catch (Exception e)
        {
            throw;
            return Response<User>.Fail($"Error during creating of user: {e.Message}");
        }
    }

    public async Task<Response<string>> Login(LoginModel model)
    {
        try
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(e => e.Nickname == model.Nickname);

            if (user == null)
            {
                return Response<string>.Fail("User with this nickname does not exist");
            }

            if (!AuthenticationHelpers.VerifyPasswordHash(model.Password,user.Password, user.Salt))
            {
                return Response<string>.Fail("Wrong password");
            }

            var token = _authenticationHelpers.GenerateToken(user.Nickname);
            
            return Response<string>.Ok(token);
        }catch (Exception e)
        {
            throw;
            return Response<string>.Fail($"Error during login: {e.Message}");
        }
    }
}