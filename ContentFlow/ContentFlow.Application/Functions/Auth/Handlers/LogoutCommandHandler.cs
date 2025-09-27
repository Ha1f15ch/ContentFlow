using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    public LogoutCommandHandler(
        IUserService userService, 
        ITokenService tokenService, 
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userService = userService;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenRepository.RevokeAllActiveByUserIdAsync(request.UserId, "User logout", cancellationToken);
        
        return true;
    }
}