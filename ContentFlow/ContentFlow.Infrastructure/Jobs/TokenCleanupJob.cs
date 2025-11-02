using ContentFlow.Application.Interfaces.RefreshToken;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Jobs;

public class TokenCleanupJob
{
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly ILogger<TokenCleanupJob> _logger;

    public TokenCleanupJob(
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<TokenCleanupJob> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }

    public async Task CleanupExpiredTokensAsync() =>
        await CleanupExpiredTokensAsync(CancellationToken.None);
    
    /// <summary>
    /// Удаляет все refresh-токены, срок действия которых истёк
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting cleanup of expired refresh tokens");

        try
        {
            var count = await _refreshTokenRepository.DeleteExpiredTokenAsync(cancellationToken);
            _logger.LogInformation("Successfully deleted {Count} expired refresh tokens", count);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting expired refresh tokens");
            throw;
        }
    }
}