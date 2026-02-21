using System.Runtime.InteropServices.JavaScript;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IRefreshTokenRepository  _refreshTokenRepository;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;
    
    public LoginCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserService userService,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }
    
    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for user: {Email} from IP: {IpAddress}, DeviceId: {DeviceId}",
            request.Email, request.Metadata.IpAddress, request.Metadata.DeviceId);
        
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        
        if (user == null || !await _userService.CheckPasswordAsync(request.Email, request.Password, cancellationToken))
        {
            _logger.LogWarning("Failed login attempt: invalid credentials for email: {Email}", request.Email);
            return new AuthResult(false, Errors: "User not found or password incorrect. Try again later");
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("Login blocked: email not confirmed for user: {Email}", request.Email);
            return new AuthResult(false, Errors: "Email is not confirmed");
        }
        
        var roles = await _userService.GetRolesAsync(user.Email, cancellationToken);
        _logger.LogInformation("User {UserId} authenticated successfully. Roles: {Roles}", user.Id, string.Join(", ", roles));
        
        var accessToken = _tokenService.GenerateToken(
            user.Id,
            user.Email!,
            roles: roles,
            user.UserName);
        
        // 1) (опционально) ревокаем старые refresh на этом девайсе
        // можно сделать отдельным методом, но минимум — найти активный и revoke
        var active = await _refreshTokenRepository.GetActiveByUserAndDeviceAsync(user.Id, request.Metadata.DeviceId, cancellationToken);
        if (active != null)
        {
            // в replacedByTokenHash запишем lookup нового (создадим ниже)
            // пока просто revoke, replaced проставим после генерации нового
            await _refreshTokenRepository.RevokeAsync(active.Id, request.Metadata.IpAddress, replacedByTokenHash: null, cancellationToken);
        }

        // 2) генерим новый refresh
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

        return new AuthResult(true, Token: accessToken, RefreshToken: refreshPlain);
    }
}