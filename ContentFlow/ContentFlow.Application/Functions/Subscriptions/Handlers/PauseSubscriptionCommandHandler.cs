using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Common;
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
    private readonly IUnitOfWork _unitOfWork;
    
    public PauseSubscriptionCommandHandler(
        ILogger<PauseSubscriptionCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(PauseSubscriptionCommand request, CancellationToken cancellationToken)
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
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Pause failed", request.FollowingProfileId);
            throw new NotFoundException($"Target UserProfile by ID {request.FollowingProfileId} not found.");
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Subscription for pause not found (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException("Subscription for pause not found");
        }
    }
}