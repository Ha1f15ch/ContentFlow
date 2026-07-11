using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class ReactivateAccountCommandHandler : IRequestHandler<ReactivateAccountCommand, AuthResult>
{
    private readonly IUserService _userService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReactivateAccountCommandHandler> _logger;

    public ReactivateAccountCommandHandler(
        IUserService userService,
        IUserProfileRepository userProfileRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReactivateAccountCommandHandler> logger)
    {
        _userService = userService;
        _userProfileRepository = userProfileRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(ReactivateAccountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Account reactivation requested for email: {Email}", request.Email);

        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !await _userService.CheckPasswordAsync(request.Email, request.Password, cancellationToken))
        {
            _logger.LogWarning("Reactivation failed: invalid credentials for email: {Email}", request.Email);
            return new AuthResult(false, Errors: "Invalid email or password.");
        }

        if (!await _userService.IsSelfDeletedAccountAsync(user.Id, cancellationToken))
        {
            _logger.LogWarning("Reactivation rejected: account is active for email: {Email}", request.Email);
            return new AuthResult(
                false,
                Errors: "Account is active.",
                Message: "This account is not deleted. Try signing in.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (profile == null)
        {
            _logger.LogError("Reactivation failed: profile not found for user {UserId}", user.Id);
            return new AuthResult(false, Errors: "Profile not found.");
        }

        await _userService.ReactivateUserAsync(user.Id, cancellationToken);
        profile.Restore();
        await _userProfileRepository.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userService.GetRolesAsync(user.Email, cancellationToken);
        var accessToken = _tokenService.GenerateToken(user.Id, user.Email!, roles, user.UserName);

        var refreshPlain = TokenGenerator.GenerateRefreshToken();
        var lookup = TokenLookup.Sha256Base64(refreshPlain);
        var (hash, salt) = PasswordHasher.Hash(refreshPlain);

        await _refreshTokenRepository.AddAsync(
            userId: user.Id,
            tokenLookupHash: lookup,
            tokenHash: hash,
            tokenSalt: salt,
            createdByIp: request.Metadata.IpAddress,
            deviceId: request.Metadata.DeviceId,
            expiresAt: DateTime.UtcNow.AddDays(7),
            ct: cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account reactivated successfully for user {UserId}", user.Id);
        return new AuthResult(
            true,
            Token: accessToken,
            RefreshToken: refreshPlain,
            Message: "Account restored successfully.");
    }
}
