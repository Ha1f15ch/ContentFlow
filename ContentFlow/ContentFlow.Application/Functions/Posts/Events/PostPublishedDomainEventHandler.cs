using ContentFlow.Application.Interfaces.Notification;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Events;

public class PostPublishedDomainEventHandler : INotificationHandler<PostPublishedNotification>
{
    private readonly ILogger<PostPublishedDomainEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public PostPublishedDomainEventHandler(ILogger<PostPublishedDomainEventHandler> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Handle(PostPublishedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.NotifyPostPublishedAsync(notification.PostId, notification.AuthorUserProfileId, cancellationToken);
            _logger.LogInformation($"Published {notification.PostId} to {notification.AuthorUserProfileId}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured");
            throw new InvalidOperationException("An error occured");
        }
    }
}