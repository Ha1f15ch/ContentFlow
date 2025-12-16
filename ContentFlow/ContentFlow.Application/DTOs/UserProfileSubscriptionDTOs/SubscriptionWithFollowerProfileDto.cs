namespace ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;

/// <summary>
/// Кто подписан
/// </summary>
public record SubscriptionWithFollowerProfileDto
{
    public int SubscriptionId { get; init; }
    public int FollowerProfileId {get; init; }
    public string? FollowerFullName { get; init; } 
    public string? FollowerAvatarUrl { get; init; } 
    public bool IsPaused { get; init; }
    public bool IsActive { get; init; }
    public DateTime SubscribedAt { get; init; }
}