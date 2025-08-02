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
    private readonly DbContext _context;

    public AuthUserService(DbContext context)
    {
        _context = context;
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
        }
        catch (Exception e)
        {
            return Response<User>.Fail($"Error during creating of user: {e.Message}");
        }
    }
}