using System.Runtime.InteropServices.JavaScript;
using ContentFlow.Application.Common;
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
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !await _userService.CheckPasswordAsync(request.Email, request.Password, cancellationToken))
        {
            return new AuthResult(false, Errors: new() {"User not found or password incorrect. Try again later"});
        }

        if (!user.EmailConfirmed)
        {
            return new AuthResult(false, Errors: new() {"Email is not confirmed"});
        }
        
        var roles = await _userService.GetRolesAsync(user.Email, cancellationToken);
        
        var accessToken = _tokenService.GenerateToken(
            user.Id,
            user.Email!,
            firstName:  user.FirstName,
            lastName:  user.LastName,
            roles: roles);
        
        var refreshToken = TokenGenerator.GenerateRefreshToken();
        var (tokenHash, tokenSalt) = PasswordHasher.Hash(refreshToken);
        
        try
        {
            await _refreshTokenRepository.AddAsync(
                userId: user.Id,
                tokenHash: tokenHash,
                tokenSalt: tokenSalt,
                createdByIp: request.Metadata.IpAddress,
                deviceId: request.Metadata.DeviceId,
                expiresAt: DateTime.UtcNow.AddDays(7),
                ct: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save refresh token: {ex.Message}");
            return new AuthResult(false, Errors: new() { "Authentication failed" });
        }
        
        return new AuthResult(
            true,
            Token: accessToken,
            RefreshToken: refreshToken,
            Errors: null);
    }
}