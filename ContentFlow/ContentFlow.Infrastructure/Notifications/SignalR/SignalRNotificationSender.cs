using ContentFlow.Application.Interfaces.Notification;
using Microsoft.AspNetCore.SignalR;

namespace ContentFlow.Infrastructure.Notifications.SignalR;

public class SignalRNotificationSender : IRealtimeNotificationSender
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    
    public async Task SendAsync(int userProfileId, string eventName, object payload, CancellationToken ct)
    {
        await _hubContext.Clients.User(userProfileId.ToString()).SendAsync(eventName, payload, ct);
    }
}