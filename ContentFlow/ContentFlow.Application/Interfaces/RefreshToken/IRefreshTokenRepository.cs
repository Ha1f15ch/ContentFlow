using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.RefreshToken;

public interface IRefreshTokenRepository
{
    Task AddAsync(int userId, string tokenHash, string tokenSalt, string createdByIp, string? deviceId, DateTime expiresAt, CancellationToken ct);
    Task<RefreshTokenDto?> GetValidByIdAsync(int tokenId, CancellationToken ct);
    Task<RefreshTokenDto?> GetValidByHashAsync(string tokenHash, CancellationToken ct);
    Task<bool> RevokeAsync(int tokenId, string revokedByIp, string? newTokenHash, CancellationToken ct);
    Task RevokeAllActiveByUserIdAsync(int userId, string reason, CancellationToken ct);
    Task<bool> ExistsByUserIdAndDeviceAsync(int userId, string? deviceId, CancellationToken ct);
    Task<int> DeleteExpiredTokenAsync(CancellationToken ct = default);
}