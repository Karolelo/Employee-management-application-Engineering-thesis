using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Repo.Core.Infrastructure;

public class AuthenticationHelpers
{
    private readonly IConfiguration _configuration;
    
    public AuthenticationHelpers(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public static byte[] GenerateSalt(int size)
    {
        byte[] salt = new byte[size];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }
    //TODO jak ja zrobiłem to sprawdzanie xdd 
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

    public static bool VerifyPasswordHash(string password, string storedHash, byte[] storedSalt)
    {
        string hashedPassword = GeneratePasswordHash(password, storedSalt);
        return hashedPassword == storedHash;
    }

    //Potem zmienić claimy, na inne niz user name
    public string GenerateToken(string username) {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, username)
            }),
            Issuer = _configuration["JWT:ValidIssuer"],
            Audience = _configuration["JWT:ValidAudience"],
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
 
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public bool ValidateToken(string token) {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]);
 
        try {
            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
 
            var jwtToken = (JwtSecurityToken)validatedToken;
            return true;
        }
        catch {
            return false;
        }
    }
    
}
