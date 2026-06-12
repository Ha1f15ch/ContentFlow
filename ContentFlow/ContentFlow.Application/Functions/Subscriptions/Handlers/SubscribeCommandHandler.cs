using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Notification;
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
    private readonly INotificationService  _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    
    public SubscribeCommandHandler(
        ILogger<SubscribeCommandHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubscribeCommandHandler.Handle . . .");

        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.FollowerUserId, cancellationToken);
        if (requesterUserProfile == null)
        {
            _logger.LogWarning("Requester user profile not found by userId = {RequesterId}. Subscribe to profile ID = {FollowingProfileId} not complete", request.FollowerUserId, request.FollowingProfileId);
            throw new NotFoundException($"Requester UserProfile by user ID {request.FollowerUserId} not found.");
        }

        var targetUserProfile = await _userProfileRepository.GetByIdAsync(request.FollowingProfileId, cancellationToken);
        if (targetUserProfile == null)
        {
            _logger.LogError("Target user profile ID = {TargetUserProfileId} not found. Subscribe failed", request.FollowingProfileId);
            throw new NotFoundException($"Target UserProfile by ID {request.FollowingProfileId} not found.");
        }

        if (requesterUserProfile.Id == targetUserProfile.Id)
        {
            _logger.LogWarning("User profile {UserProfileId} tried to subscribe to itself", requesterUserProfile.Id);
            throw new InvalidOperationException("Cannot subscribe to yourself.");
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _logger.LogInformation("Create new Subscription (from {Follower} to {Following})", requesterUserProfile.Id, targetUserProfile.Id);

            await _subscriptionRepository.AddAsync(requesterUserProfile.Id, targetUserProfile.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        await _notificationService.NotifyUserSubscribedAsync(
            requesterUserProfile.Id,
            targetUserProfile.Id,
            cancellationToken);
        
        return Unit.Value;
    }
}