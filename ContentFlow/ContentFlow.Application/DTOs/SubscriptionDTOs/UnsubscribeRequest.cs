namespace ContentFlow.Application.DTOs.SubscriptionDTOs;

/// <summary>
/// От кого отписываемся
/// </summary>
public record UnsubscribeRequest(int FollowingId);