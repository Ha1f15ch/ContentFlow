using ContentFlow.Application.Interfaces.Notification;
using Microsoft.AspNetCore.SignalR;

namespace ContentFlow.Infrastructure.Notifications.SignalR;

public class SignalRNotificationSender : IRealtimeNotificationSender
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public SignalRNotificationSender(IHubContext<NotificationsHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendAsync(int userId, string eventName, object payload, CancellationToken ct)
    {
        await _hubContext.Clients.User(userId.ToString()).SendAsync(eventName, payload, ct);
    }
}