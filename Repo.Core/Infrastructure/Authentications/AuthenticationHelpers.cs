using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Models.auth;

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
    public string GenerateToken(int userId,string userName,ICollection<string> RoleName = null) {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]);
        
        var claims = new List<Claim>
        {
            new Claim("id",userId.ToString()),
            new Claim(ClaimTypes.Name, userName)
        };
        
        claims.AddRange(RoleName.Select(role => new Claim(ClaimTypes.Role, role)));
        
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
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

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
    
    public TokenModel GenerateTokens(int userId,string username,ICollection<string> RoleName = null)
    {
        return new TokenModel
        {
            AccessToken = GenerateToken(userId,username,RoleName),
            RefreshToken = GenerateRefreshToken()
        };
    }
    
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false 
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            
            return principal;
        }
        catch
        {
            return null;
        }
    }
    
   
    
}
