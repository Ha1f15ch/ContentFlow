using System.Text.Json;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.NotificationDTOs;

public record NotificationDto(
    int Id,
    NotificationType Type,
    JsonElement Payload,
    bool IsRead,
    DateTime CreatedAt);
