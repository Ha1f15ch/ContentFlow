using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, Unit>
{
    private readonly ILogger<SubscribeCommandHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public SubscribeCommandHandler(
        ILogger<SubscribeCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubscribeCommandHandler.Handle . . .");

        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            _logger.LogWarning("Requester user profile not founded by userId = {RequesterId}. Subscribe to user profile ID = {UserProfile} not complete", request.FollowerUserId, request.FollowingUserId);
            return Unit.Value;
        }

        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingUserId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Subscribe failed}", request.FollowingUserId);
            throw new NotFoundException($"Target User profile by ID {request.FollowingUserId} for subscribe to him not founded");
        }
        
        // проверяем, был ли пользователь подписан ранее
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription != null)
        {
            if (subscription.IsActive)
            {
                _logger.LogInformation("Subscription (from {Follower} to {Following}) is active", requesterUserProfile.Id, targetUserProfile.Id);
                return Unit.Value;
            }
            
            _logger.LogInformation("Subscription exist. Reactivate subscription (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            
            subscription.Reactivate();
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Create new Subscription (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);

            await _subscriptionRepository.AddAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}