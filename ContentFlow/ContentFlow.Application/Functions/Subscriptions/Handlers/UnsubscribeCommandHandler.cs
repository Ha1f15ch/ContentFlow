using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, Unit>
{
    private readonly ILogger<UnsubscribeCommandHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public UnsubscribeCommandHandler(
        ILogger<UnsubscribeCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UnsubscribeCommandHandler.Handle . . .");
        
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            _logger.LogInformation("UserProfile (requester) not found. ID: {RequesterUserId}", request.FollowerUserId);
            return Unit.Value;
        }
        
        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingUserId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("UserProfile (target) not found. ID: {TargetUserId}", request.FollowingUserId);
            throw new NotFoundException($"UserProfile (target) not found. ID: {request.FollowingUserId}");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription == null)
        {
            _logger.LogError("Subscription (from {Follower} to {Following}) not found", request.FollowerUserId, request.FollowingUserId);
            throw new NotFoundException($"Subscription from {request.FollowerUserId} to {request.FollowingUserId} not found");
        }
        
        subscription.Deactivate();
        await _subscriptionRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Subscription {SubscriptionId} has been deleted", subscription.Id);
        
        return Unit.Value;
    }
}