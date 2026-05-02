using System.Text.Json;
using ContentFlow.Application.DTOs.NotificationDTOs;
using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Application.Security;
using ContentFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationsController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int take = 20, CancellationToken ct = default)
    {
        var userId = User.GetAuthenticatedUserId();
        var normalizedTake = Math.Clamp(take, 1, 50);
        var notifications = await _notificationRepository.GetByUserAsync(userId, normalizedTake, ct);

        return Ok(notifications.Select(ToDto));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();
        var unreadNotifications = await _notificationRepository.GetUnreadByUserAsync(userId, ct);

        return Ok(new { unreadCount = unreadNotifications.Count });
    }

    [HttpPost("{notificationId:int}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId, CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();
        var notification = await _notificationRepository.GetByIdForUserAsync(notificationId, userId, ct);

        if (notification == null)
            return NotFound();

        notification.MarkRead();
        await _notificationRepository.SaveChangesAsync(ct);

        return NoContent();
    }

    private static NotificationDto ToDto(Notification notification)
    {
        var payload = JsonSerializer.Deserialize<JsonElement>(notification.Payload);

        return new NotificationDto(
            notification.Id,
            notification.Type,
            payload,
            notification.IsRead,
            notification.CreatedAt);
    }
}
