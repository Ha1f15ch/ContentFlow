using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshCommandHandler> _logger;

    public RefreshCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserService userService,
        ITokenService tokenService,
        ILogger<RefreshCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(RefreshCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return new AuthResult(false, Errors: "Refresh token is missing");

        var lookupHash = TokenLookup.Sha256Base64(request.RefreshToken);

        var stored = await _refreshTokenRepository.GetActiveByLookupHashAsync(lookupHash, ct);
        if (stored == null)
            return new AuthResult(false, Errors: "Invalid refresh token");

        if (!PasswordHasher.Verify(request.RefreshToken, stored.TokenSalt, stored.TokenHash))
            return new AuthResult(false, Errors: "Invalid refresh token");

        var user = await _userService.GetByIdAsync(stored.UserId, ct);
        if (user == null)
            return new AuthResult(false, Errors: "User not found");

        var roles = await _userService.GetRolesAsync(user.Email!, ct);

        var newAccess = _tokenService.GenerateToken(
            user.Id,
            user.Email!,
            roles: roles,
            user.UserName);

        // rotation
        var newRefreshPlain = TokenGenerator.GenerateRefreshToken();
        var newLookupHash = TokenLookup.Sha256Base64(newRefreshPlain);
        var (newHash, newSalt) = PasswordHasher.Hash(newRefreshPlain);

        var revoked = await _refreshTokenRepository.RevokeAsync(
            tokenId: stored.Id,
            revokedByIp: request.Metadata.IpAddress,
            replacedByTokenHash: newLookupHash,
            ct: ct);

        if (!revoked)
            return new AuthResult(false, Errors: "Failed to revoke refresh token");

        await _refreshTokenRepository.AddAsync(
            userId: user.Id,
            tokenLookupHash: newLookupHash,
            tokenHash: newHash,
            tokenSalt: newSalt,
            createdByIp: request.Metadata.IpAddress,
            deviceId: stored.DeviceId ?? request.Metadata.DeviceId,
            expiresAt: DateTime.UtcNow.AddDays(7),
            ct: ct);

        return new AuthResult(true, Token: newAccess, RefreshToken: newRefreshPlain);
    }
}