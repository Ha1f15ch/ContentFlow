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
    
    public async Task AddAsync(int userId, string tokenHash, string tokenSalt, string createdByIp, string? deviceId, DateTime expiresAt,
        CancellationToken ct)
    {
        try
        {
            var newToken = new RefreshToken
            {
                UserId = userId,
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
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при выполнении создания refreshToken. {e.Message}");
            throw;
        }
    }

    public async Task<RefreshTokenDto?> GetValidByIdAsync(int tokenId, CancellationToken ct)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t =>
                t.Id == tokenId &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, ct
            );
        
        return token == null ? null : GetTokenDto(token);
    }

    public async Task<RefreshTokenDto?> GetValidByHashAsync(string tokenHash, CancellationToken ct)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t =>
                t.TokenHash == tokenHash &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow, ct
            );
        
        return token == null ? null : GetTokenDto(token);
    }

    public async Task<bool> RevokeAsync(int tokenId, string revokedByIp, string? newTokenHash, CancellationToken ct)
    {
        var token = await _context.RefreshTokens.FindAsync([tokenId], ct);
        if (token == null) return false;
        
        token.IsRevoked = true;
        token.ExpiresAt = DateTime.UtcNow;
        token.RevokedByIp = revokedByIp;
        token.ReplacedByTokenHash = newTokenHash;
        
        _context.RefreshTokens.Update(token);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> ExistsByUserIdAndDeviceAsync(int userId, string? deviceId, CancellationToken ct)
    {
        return await _context.RefreshTokens
            .AnyAsync(t =>
                t.UserId == userId &&
                t.DeviceId == deviceId &&
                !t.IsRevoked && 
                t.ExpiresAt > DateTime.UtcNow, ct
            );
    }

    private static RefreshTokenDto GetTokenDto(RefreshToken token)
    {
        return new RefreshTokenDto
        (
            Id: token.Id,
            UserId: token.UserId,
            TokenHash: token.TokenHash,
            ExpiresAt: token.ExpiresAt,
            CreatedAt: token.CreatedAt,
            CreatedByIp: token.CreatedByIp,
            IsRevoked: token.IsRevoked,
            DeviceId: token.DeviceId
        );
    }
}