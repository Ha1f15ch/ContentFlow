using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.RefreshToken;

public interface IRefreshTokenRepository
{
    Task AddAsync(int userId, string tokenLookupHash, string tokenHash, string tokenSalt, string createdByIp, string? deviceId, DateTime expiresAt, CancellationToken ct);
    Task<RefreshTokenDto?> GetActiveByLookupHashAsync(string lookupHash, CancellationToken ct);

    Task<bool> RevokeAsync(int tokenId, string revokedByIp, string? replacedByTokenHash, CancellationToken ct);

    Task<RefreshTokenDto?> GetActiveByUserAndDeviceAsync(int userId, string? deviceId, CancellationToken ct);
    Task RevokeAllActiveByUserIdAsync(int userId, string reason, CancellationToken ct);
    Task<bool> ExistsByUserIdAndDeviceAsync(int userId, string? deviceId, CancellationToken ct);
    Task<int> DeleteExpiredTokenAsync(CancellationToken ct = default);
    Task<List<RefreshTokenDto>> GetRefreshTokenByUserid(int userId, CancellationToken ct);
}