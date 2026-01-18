using ContentFlow.Application.Interfaces.Common.Jobs;
using ContentFlow.Application.Interfaces.Notification;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Jobs;

public class NotificationSenderJob : INotificationSenderJob
{
    private readonly IRealtimeNotificationSender _realtimeNotificationSender;
    private readonly ILogger<NotificationSenderJob> _logger;

    public NotificationSenderJob(
        IRealtimeNotificationSender realtimeNotificationSender,
        ILogger<NotificationSenderJob> logger)
    {
        _realtimeNotificationSender = realtimeNotificationSender;
        _logger = logger;
    }
    
    public async Task SendBatchAsync(List<int> userIds, int postId, int authorId, CancellationToken ct)
    {
        var tasks = userIds.Select(userId => 
            _realtimeNotificationSender.SendAsync(userId, "PostPublished", new {postId, authorId}, ct));
        
        await Task.WhenAll(tasks);
        
        _logger.LogInformation("Sent batch of {Count} notifications for Post {PostId}", userIds.Count, postId);
    }
}