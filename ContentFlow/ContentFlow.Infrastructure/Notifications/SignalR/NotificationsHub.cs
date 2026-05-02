using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ContentFlow.Infrastructure.Notifications.SignalR;

[Authorize]
public class NotificationsHub : Hub
{
    
}