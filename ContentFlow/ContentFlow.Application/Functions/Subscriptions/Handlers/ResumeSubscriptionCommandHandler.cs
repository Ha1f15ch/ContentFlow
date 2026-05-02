using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class ResumeSubscriptionCommandHandler : IRequestHandler<ResumeSubscriptionCommand, Unit>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<ResumeSubscriptionCommandHandler> _logger;
    
    public ResumeSubscriptionCommandHandler(
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<ResumeSubscriptionCommandHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(ResumeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ResumeSubscriptionCommandHandler.Handle . . .");
        
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            throw new NotFoundException($"Requester UserProfile by user ID {request.FollowerUserId} not found.");
        }
        
        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingProfileId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Resume failed", request.FollowingProfileId);
            throw new NotFoundException($"Target UserProfile by ID {request.FollowingProfileId} not found.");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription != null)
        {
            if (!subscription.IsPaused)
            {
                _logger.LogInformation("Subscription (from {Follower} to {Following}) is active and non paused", requesterUserProfile.Id, targetUserProfile.Id);
                return Unit.Value;
            }
            
            _logger.LogInformation("Subscription exist. Resume subscription (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            
            subscription.Resume();
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Subscription for resume not found (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException("Subscription for resume not found");
        }
    }
}