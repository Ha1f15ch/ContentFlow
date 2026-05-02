using System.Text.Json;
using ContentFlow.Application.Interfaces.Common.Jobs;
using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IRealtimeNotificationSender _realtimeNotificationSender;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserService _userService;
    private readonly ILogger<NotificationService> _logger;
    
    public NotificationService(
        INotificationRepository notificationRepository, 
        ISubscriptionRepository subscriptionRepository, 
        ILogger<NotificationService> logger, 
        IRealtimeNotificationSender realtimeNotificationSender,
        IUserProfileRepository userProfileRepository,
        IUserService userService)
    {
        _notificationRepository = notificationRepository;
        _subscriptionRepository = subscriptionRepository;
        _realtimeNotificationSender = realtimeNotificationSender;
        _userProfileRepository = userProfileRepository;
        _userService = userService;
        _logger = logger;
    }
    
    public async Task NotifyPostPublishedAsync(int postId, int authorProfileId, CancellationToken ct)
    {
        var subscriberUserIds = await _subscriptionRepository.GetUserIdsWithActiveNotification(authorProfileId, ct);
        var authorProfile = await _userProfileRepository.GetByIdAsync(authorProfileId, ct);
        var author = authorProfile == null
            ? null
            : await _userService.GetByIdAsync(authorProfile.UserId, ct);

        var payload = new
        {
            postId,
            authorProfileId,
            authorUserId = authorProfile?.UserId,
            authorUserName = author?.UserName,
            authorAvatarUrl = author?.AvatarUrl
        };
        
        var notifications = subscriberUserIds.Select(userId => 
            new Notification(
                userId,
                NotificationType.NewPost,
                JsonSerializer.Serialize(payload)))
            .ToList();
        
        await _notificationRepository.AddRangeAsync(notifications, ct);
        await _notificationRepository.SaveChangesAsync(ct);
        
        int batchSize = 1000; // число пакетов
        for (int i = 0; i < subscriberUserIds.Count; i += batchSize)
        {
            var batch = subscriberUserIds.Skip(i).Take(batchSize).ToList();
            // Попытка распараллелить через job hangfire
            Hangfire.BackgroundJob.Enqueue<INotificationSenderJob>(
                job => job.SendBatchAsync(batch, postId, authorProfileId, CancellationToken.None));
        }
        
        _logger.LogInformation("Notifications for post {PostId} by author profile {AuthorProfileId} were published.", postId, authorProfileId);
    }

    public async Task NotifyUserSubscribedAsync(int followerProfileId, int followingProfileId, CancellationToken ct)
    {
        var targetUserProfile = await _userProfileRepository.GetByIdAsync(followingProfileId, ct);
        var followerProfile = await _userProfileRepository.GetByIdAsync(followerProfileId, ct);

        if (targetUserProfile == null || followerProfile == null)
        {
            _logger.LogWarning(
                "Cannot create subscription notification. Follower profile {FollowerProfileId} or following profile {FollowingProfileId} was not found.",
                followerProfileId,
                followingProfileId);
            throw new InvalidOperationException("Follower profile or following profile does not exist.");
        }

        var follower = await _userService.GetByIdAsync(followerProfile.UserId, ct);

        var payload = new
        {
            followerProfileId = followerProfile.Id,
            followerUserId = followerProfile.UserId,
            followerUserName = follower.UserName,
            followerAvatarUrl = follower.AvatarUrl
        };

        var notification = new Notification(
            userId: targetUserProfile.UserId,
            type: NotificationType.NewSubscriber,
            payload: JsonSerializer.Serialize(payload));
        
        await _notificationRepository.AddAsync(notification, ct);
        await _realtimeNotificationSender.SendAsync(
            targetUserProfile.UserId,
            "NewSubscriber",
            payload,
            ct);
    }
}