using Microsoft.EntityFrameworkCore;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Models;
using Repo.Server.AuthModule.Interfaces;
using Task = System.Threading.Tasks.Task;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MyDbContext _context;
    private readonly Dictionary<int, RefreshToken> _usersTokensCache;
    
    public RefreshTokenRepository(MyDbContext context)
    {
        _context = context;
        _usersTokensCache = new Dictionary<int, RefreshToken>();
        InitializeCache();
    }
    
    public async Task<RefreshToken?> FindByUserId(int userId)
    {
        // Check cache first
        if (_usersTokensCache.TryGetValue(userId, out var cachedToken) && IsTokenValid(cachedToken))
        {
            return cachedToken;
        }
        
        // Fetch from database - get only valid, non-revoked tokens
        var token = await _context.RefreshTokens
            .Where(t => t.User_ID == userId 
                        && t.RevokedAt == null 
                        && t.ExpireDate > DateTime.Now)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
        
        // Update cache
        if (token != null)
        {
            _usersTokensCache[userId] = token;
        }
        else
        {
            _usersTokensCache.Remove(userId);
        }
        
        return token;
    }
    
    public async Task<RefreshToken> Add(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        
        // Update cache
        _usersTokensCache[refreshToken.User_ID] = refreshToken;
        
        return refreshToken;
    }
    
    public async Task<RefreshToken> Update(RefreshToken refreshToken)
    {
        await _context.RefreshTokens
            .Where(t => t.ID == refreshToken.ID)
            .ExecuteUpdateAsync(t => t
                .SetProperty(p => p.Token, refreshToken.Token)
                .SetProperty(p => p.ExpireDate, refreshToken.ExpireDate)
                .SetProperty(p => p.RevokedAt, refreshToken.RevokedAt));
        
        // Remove from cache if revoked
        if (refreshToken.RevokedAt.HasValue)
        {
            _usersTokensCache.Remove(refreshToken.User_ID);
        }
        
        return refreshToken;
    }
    
    public async Task Delete(int tokenId)
    {
        var token = await _context.RefreshTokens.FindAsync(tokenId);
        if (token != null)
        {
            _usersTokensCache.Remove(token.User_ID);
        }
        
        await _context.RefreshTokens
            .Where(t => t.ID == tokenId)
            .ExecuteDeleteAsync();
    }
    
    private void InitializeCache()
    {
        try
        {
            _usersTokensCache.Clear();
            
            var validTokens = _context.Users
                .Include(u => u.RefreshTokens)
                .AsEnumerable()
                .Select(u => new
                {
                    UserId = u.ID,
                    Token = u.RefreshTokens
                        .Where(IsTokenValid)
                        .OrderByDescending(t => t.CreatedAt)
                        .FirstOrDefault()
                })
                .Where(x => x.Token != null)
                .ToDictionary(x => x.UserId, x => x.Token!);
            
            foreach (var kvp in validTokens)
            {
                _usersTokensCache[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception)
        {
            // If initialization fails, continue with empty cache
            // This prevents application startup failures
        }
    }
    
    private static bool IsTokenValid(RefreshToken? token)
    {
        return token is not null 
               && token.ExpireDate > DateTime.Now 
               && token.RevokedAt is null
               && token.CreatedAt <= DateTime.Now;
    }
}