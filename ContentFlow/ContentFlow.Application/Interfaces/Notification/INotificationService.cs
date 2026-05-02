namespace ContentFlow.Application.Interfaces.Notification;

public interface INotificationService
{
    /// <summary>
    /// Сохраняет запись уведомления в БД
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="authorProfileId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task NotifyPostPublishedAsync(int postId, int authorProfileId, CancellationToken ct);
    
    Task NotifyUserSubscribedAsync(int followerProfileId, int followingProfileId, CancellationToken ct);
}