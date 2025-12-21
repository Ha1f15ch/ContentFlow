using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class PauseSubscriptionCommandHandler : IRequestHandler<PauseSubscriptionCommand, Unit>
{
    private readonly ILogger<PauseSubscriptionCommandHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public PauseSubscriptionCommandHandler(
        ILogger<PauseSubscriptionCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(PauseSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ResumeSubscriptionCommandHandler.Handle . . .");
        
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            return Unit.Value;
        }
        
        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingUserId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Subscribe failed}", request.FollowingUserId);
            throw new NotFoundException($"Target User profile by ID {request.FollowingUserId} for subscribe to him not founded");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription != null)
        {
            if (subscription.IsPaused)
            {
                _logger.LogInformation("Subscription (from {Follower} to {Following}) is active and paused", requesterUserProfile.Id, targetUserProfile.Id);
                return Unit.Value;
            }
            
            _logger.LogInformation("Subscription exist. Pause subscription (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            
            subscription.Pause();
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Subscription for pause not found (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException("Subscription for pause not found");
        }
    }
}