using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class DisableNotificationsCommandHandler : IRequestHandler<DisableNotificationsCommand, Unit>
{
    private readonly ILogger<DisableNotificationsCommandHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DisableNotificationsCommandHandler(
        ILogger<DisableNotificationsCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DisableNotificationsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DisableNotificationsCommandHandler.Handle . . . ");
        
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            _logger.LogWarning("Requester user profile not found by userId = {RequesterId}. Disable notify to profile ID = {FollowingProfileId} not complete", request.FollowerUserId, request.FollowingProfileId);
            throw new NotFoundException($"Requester UserProfile by user ID {request.FollowerUserId} not found.");
        }

        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingProfileId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Disable notify not complete", request.FollowingProfileId);
            throw new NotFoundException($"Target UserProfile by ID {request.FollowingProfileId} not found.");
        }
        
        var subscription = await _subscriptionRepository.GetByFollowerAndFollowingAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);

        if (subscription != null)
        {
            if (!subscription.NotificationsEnabled)
            {
                _logger.LogInformation("Subscription (from {Follower} to {Following}) is active and notify disabled", requesterUserProfile.Id, targetUserProfile.Id);
                return Unit.Value;
            }
            
            _logger.LogInformation("Subscription exist. Notify is active (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            
            subscription.DisableNotifications();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
        else
        {
            _logger.LogInformation("Subscription for disable notify not found (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);
            throw new NotFoundException("Subscription for disable notify not found");
        }
    }
}