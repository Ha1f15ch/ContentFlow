namespace ContentFlow.Application.Interfaces.Notification;

public interface INotificationRepository
{
    Task AddAsync(Domain.Entities.Notification notification, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Notification>> GetUnreadByUserAsync(int userId, CancellationToken ct);
    public Task AddRangeAsync(IEnumerable<Domain.Entities.Notification> notifications, CancellationToken ct);
    public Task SaveChangesAsync(CancellationToken ct);
}