namespace ContentFlow.Application.DTOs.SubscriptionDTOs;

public record SubscriptionInfoDto
{
    public bool IsActive { get; init; } = false;
    public bool IsPaused { get; init; }  = false;
    public bool NotificationsEnabled { get; init; } = false;
    public DateTime? SubscribedAt { get; init; }
};