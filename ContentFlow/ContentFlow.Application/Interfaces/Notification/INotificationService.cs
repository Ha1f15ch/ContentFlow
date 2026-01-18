using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Interfaces.Notification;

public interface INotificationService
{
    /// <summary>
    /// Сохраняет запись уведомления в БД
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="authorId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task NotifyPostPublishedAsync(int postId, int authorId, CancellationToken ct);
}