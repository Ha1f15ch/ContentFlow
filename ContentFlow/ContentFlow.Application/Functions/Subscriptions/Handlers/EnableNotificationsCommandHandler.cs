using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class EnableNotificationsCommandHandler : IRequestHandler<EnableNotificationsCommand, Unit>
{
    private readonly ILogger<EnableNotificationsCommandHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public EnableNotificationsCommandHandler(
        ILogger<EnableNotificationsCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(EnableNotificationsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("EnableNotificationsCommandHandler.Handle . . . ");
        
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            _logger.LogWarning("Requester user profile not founded by userId = {RequesterId}. Enable notify to user profile ID = {UserProfile} not complete", request.FollowerUserId, request.FollowingUserId);
            return Unit.Value;
        }

        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingUserId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Enable notify not complete", request.FollowingUserId);
            throw new NotFoundException($"Target User profile by ID {request.FollowingUserId} not founded");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription != null)
        {
            if (subscription.NotificationsEnabled)
            {
                _logger.LogInformation("Subscription (from {Follower} to {Following}) is active and notify active", requesterUserProfile.Id, targetUserProfile.Id);
                return Unit.Value;
            }
            
            _logger.LogInformation("Subscription exist. Notify is not active (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            
            subscription.EnableNotifications();
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Subscription for enable notify not found (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException("Subscription for enable notify not found");
        }
    }
}