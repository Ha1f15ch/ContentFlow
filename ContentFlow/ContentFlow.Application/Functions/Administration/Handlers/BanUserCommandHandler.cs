using ContentFlow.Application.Functions.Administration.Commands;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Administration.Handlers;

public class BanUserCommandHandler : IRequestHandler<BanUserCommand, Unit>
{
    private readonly IUserService _userService;
    private readonly ILogger<BanUserCommandHandler> _logger;
    
    public BanUserCommandHandler(IUserService userService, ILogger<BanUserCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<Unit> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Admin user {RequesterId} requested to ban user {UserId}. Reason: '{BanReason}'",
            request.RequesterId, request.UserId, request.BanReason);

        if (string.IsNullOrWhiteSpace(request.BanReason))
        {
            _logger.LogWarning("Ban request for user {UserId} rejected: no reason provided", request.UserId);
            throw new ArgumentException("Ban reason is required.", nameof(request.BanReason));
        }

        try
        {
            await _userService.BanUserAsync(request.UserId, request.BanReason, request.RequesterId, cancellationToken);
            
            _logger.LogInformation(
                "User {UserId} successfully banned by admin {RequesterId}",
                request.UserId, request.RequesterId);
                
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ban user {UserId}", request.UserId);
            throw;
        }
    }
}