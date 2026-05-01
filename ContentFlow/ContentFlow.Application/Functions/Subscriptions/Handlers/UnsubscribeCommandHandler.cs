using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
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
            throw new NotFoundException($"Requester UserProfile by user ID {request.FollowerUserId} not found.");
        }
        
        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingProfileId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("UserProfile (target) not found. ID: {FollowingProfileId}", request.FollowingProfileId);
            throw new NotFoundException($"Target UserProfile by ID {request.FollowingProfileId} not found.");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription == null)
        {
            _logger.LogError("Subscription (from {FollowerProfileId} to {FollowingProfileId}) not found", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException($"Subscription from profile {requesterUserProfile.Id} to profile {targetUserProfile.Id} not found.");
        }
        
        subscription.Deactivate();
        await _subscriptionRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Subscription {SubscriptionId} has been deleted", subscription.Id);
        
        return Unit.Value;
    }
}