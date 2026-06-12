using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LogoutCommandHandler> _logger;
    
    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<LogoutCommandHandler> logger)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout started by user {user}", request.UserId);
        
        await _refreshTokenRepository.RevokeAllActiveByUserIdAsync(request.UserId, "User logout", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Logout completed successfully for user: {user}", request.UserId);
        return true;
    }
}