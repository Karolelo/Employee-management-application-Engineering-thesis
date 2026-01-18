using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Core.Models.api;
using Repo.Core.Models.profile;

namespace Repo.Server.ProfileModule.Services;

public class UserProfileService : IUserProfileService
{
    private readonly MyDbContext _context;

    public UserProfileService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<Response<UserProfileDTO>> GetProfileAsync(int userId)
    {
        var user = await _context.Set<User>()
            .AsNoTracking()
            .Where(u => u.ID == userId && u.Deleted == 0)
            .Select(u => new UserProfileDTO
            {
                Nickname = u.Nickname,
                Name = u.Name,
                Surname = u.Surname,
                Login = u.Login,
                Email = u.Email
            })
            .FirstOrDefaultAsync();

        return user == null
            ? Response<UserProfileDTO>.Fail("User not found")
            : Response<UserProfileDTO>.Ok(user);
    }

    public async Task<Response<object>> ChangeEmailAsync(int userId, ChangeEmailDTO model)
    {
        var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.ID == userId && u.Deleted == 0);
        if (user == null)
            return Response<object>.Fail("User not found");

        if (!AuthenticationHelpers.VerifyPasswordHash(model.CurrentPassword, user.Password, user.Salt))
            return Response<object>.Fail("Wrong password");

        var emailTaken = await _context.Set<User>()
            .AnyAsync(u => u.ID != userId && u.Email == model.NewEmail && u.Deleted == 0);
        if (emailTaken)
            return Response<object>.Fail("Email already in use");

        user.Email = model.NewEmail.Trim();
        await _context.SaveChangesAsync();

        return Response<object>.Ok(new { Message = "Email updated" });
    }

    public async Task<Response<object>> ChangePasswordAsync(int userId, ChangePasswordDTO model)
    {
        var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.ID == userId && u.Deleted == 0);
        if (user == null)
            return Response<object>.Fail("User not found");

        if (!AuthenticationHelpers.VerifyPasswordHash(model.CurrentPassword, user.Password, user.Salt))
            return Response<object>.Fail("Wrong password");

        var newSalt = AuthenticationHelpers.GenerateSalt(64);
        user.Salt = newSalt;
        user.Password = AuthenticationHelpers.GeneratePasswordHash(model.NewPassword, newSalt);

        await _context.SaveChangesAsync();
        return Response<object>.Ok(new { Message = "Password updated" });
    }
}
