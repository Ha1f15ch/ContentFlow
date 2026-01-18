namespace ContentFlow.Application.Interfaces.Notification;

public interface IRealtimeNotificationSender
{
    Task SendAsync(int userProfileId, string eventName, object payload, CancellationToken ct);
}