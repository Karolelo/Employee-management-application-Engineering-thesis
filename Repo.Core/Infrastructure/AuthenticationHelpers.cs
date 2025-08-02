using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Repo.Core.Infrastructure;

public class AuthenticationHelpers
{
    public static byte[] GenerateSalt(int size)
    {
        byte[] salt = new byte[size];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public static string GeneratePasswordHash(string password, byte[] salt)
    {
        string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        return hashedPassword;
    }

    public static bool ComparerPasswordHash(string password, string hashedPassword)
    {
        return hashedPassword == GeneratePasswordHash(password, Convert.FromBase64String(hashedPassword));
    }
}
