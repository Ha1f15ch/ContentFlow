using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Infrastructure.DatabaseEngine;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(
        int userId,
        string tokenLookupHash,
        string tokenHash,
        string tokenSalt,
        string createdByIp,
        string? deviceId,
        DateTime expiresAt,
        CancellationToken ct)
    {
        var newToken = new RefreshToken
        {
            UserId = userId,
            TokenLookupHash = tokenLookupHash,
            TokenHash = tokenHash,
            TokenSalt = tokenSalt,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = createdByIp,
            DeviceId = deviceId,
            IsRevoked = false
        };

        await _context.RefreshTokens.AddAsync(newToken, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<RefreshTokenDto?> GetActiveByLookupHashAsync(string lookupHash, CancellationToken ct)
    {
        var token = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t =>
                t.TokenLookupHash == lookupHash &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, ct);

        return token == null ? null : GetTokenDto(token);
    }

    public async Task<RefreshTokenDto?> GetActiveByUserAndDeviceAsync(int userId, string? deviceId, CancellationToken ct)
    {
        var token = await _context.RefreshTokens
            .AsNoTracking()
            .Where(t =>
                t.UserId == userId &&
                t.DeviceId == deviceId &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return token == null ? null : GetTokenDto(token);
    }

    public async Task<bool> RevokeAsync(int tokenId, string revokedByIp, string? replacedByTokenHash, CancellationToken ct)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Id == tokenId, ct);
        if (token == null) return false;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = revokedByIp;
        token.ReplacedByTokenHash = replacedByTokenHash;

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task RevokeAllActiveByUserIdAsync(int userId, string reason, CancellationToken ct)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = reason;
            token.ReplacedByTokenHash = null;
        }

        _context.RefreshTokens.UpdateRange(tokens);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByUserIdAndDeviceAsync(int userId, string? deviceId, CancellationToken ct)
    {
        return await _context.RefreshTokens.AnyAsync(t =>
            t.UserId == userId &&
            t.DeviceId == deviceId &&
            !t.IsRevoked &&
            t.ExpiresAt > DateTime.UtcNow, ct);
    }

    public async Task<int> DeleteExpiredTokenAsync(CancellationToken ct = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        if (!expiredTokens.Any()) return 0;

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(ct);
        return expiredTokens.Count;
    }

    public async Task<List<RefreshTokenDto>> GetRefreshTokenByUserid(int userId, CancellationToken ct)
    {
        var refreshTokens = await _context.RefreshTokens.Where(el => el.UserId == userId).ToListAsync(ct);
        return refreshTokens.Select(GetTokenDto).ToList();
    }

    private static RefreshTokenDto GetTokenDto(RefreshToken token)
    {
        return new RefreshTokenDto(
            Id: token.Id,
            UserId: token.UserId,
            TokenHash: token.TokenHash,
            TokenSalt: token.TokenSalt,
            TokenLookupHash: token.TokenLookupHash,
            ExpiresAt: token.ExpiresAt,
            CreatedAt: token.CreatedAt,
            CreatedByIp: token.CreatedByIp,
            IsRevoked: token.IsRevoked,
            DeviceId: token.DeviceId
        );
    }
}