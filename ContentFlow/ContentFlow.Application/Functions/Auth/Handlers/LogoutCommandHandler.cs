using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<LogoutCommandHandler> _logger;
    
    public LogoutCommandHandler(
        IUserService userService, 
        ITokenService tokenService, 
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<LogoutCommandHandler> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }
    
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout started by user {user}", request.UserId);
        
        await _refreshTokenRepository.RevokeAllActiveByUserIdAsync(request.UserId, "User logout", cancellationToken);
        
        // todo Escape from all devices. (not implemented)
        
        _logger.LogInformation("Logout completed successfully for user: {user}", request.UserId);
        return true;
    }
}