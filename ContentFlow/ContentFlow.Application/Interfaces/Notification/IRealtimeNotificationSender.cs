namespace ContentFlow.Application.Interfaces.Notification;

public interface IRealtimeNotificationSender
{
    Task SendAsync(int userId, string eventName, object payload, CancellationToken ct);
}