using System.Text.Json;
using ContentFlow.Application.Interfaces.Common.Jobs;
using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IRealtimeNotificationSender _realtimeNotificationSender;
    private readonly ILogger<NotificationService> _logger;
    
    public NotificationService(
        INotificationRepository notificationRepository, 
        ISubscriptionRepository subscriptionRepository, 
        ILogger<NotificationService> logger, 
        IRealtimeNotificationSender realtimeNotificationSender)
    {
        _notificationRepository = notificationRepository;
        _subscriptionRepository = subscriptionRepository;
        _realtimeNotificationSender = realtimeNotificationSender;
        _logger = logger;
    }
    
    public async Task NotifyPostPublishedAsync(int postId, int authorId, CancellationToken ct)
    {
        var subscriberUserIds = await _subscriptionRepository.GetUserIdsWithActiveNotification(authorId, ct);
        
        var notifications = subscriberUserIds.Select(userProfileId => 
            new Notification(
                userProfileId,
                NotificationType.NewPost,
                JsonSerializer.Serialize(new {postId, authorId})))
            .ToList();
        
        await _notificationRepository.AddRangeAsync(notifications, ct);
        await _notificationRepository.SaveChangesAsync(ct);
        
        int batchSize = 1000; // число пакетов
        for (int i = 0; i < subscriberUserIds.Count; i += batchSize)
        {
            var batch = subscriberUserIds.Skip(i).Take(batchSize).ToList();
            // Попытка распараллелить через job hangfire
            Hangfire.BackgroundJob.Enqueue<INotificationSenderJob>(
                job => job.SendBatchAsync(batch, postId, authorId, CancellationToken.None));
        }
        
        _logger.LogInformation($"Notifications for post id = {postId}, by user id = {authorId} were published.");
    }
}