using ContentFlow.Application.Functions.Administration.Commands;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Administration.Handlers;

public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, Unit>
{
    private readonly ILogger<UnbanUserCommandHandler> _logger;
    private readonly IUserService _userService;
    
    public UnbanUserCommandHandler(IUserService userService, ILogger<UnbanUserCommandHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Admin user {RequesterId} requested to unban user {UserId}",
            request.RequesterId, request.UserId);

        try
        {
            await _userService.UnbanUserAsync(request.UserId, request.RequesterId, cancellationToken);
            
            _logger.LogInformation(
                "User {UserId} successfully unbanned by admin {RequesterId}",
                request.UserId, request.RequesterId);
                
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unban user {UserId}", request.UserId);
            throw;
        }
    }
}