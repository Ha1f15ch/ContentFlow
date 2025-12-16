namespace ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;

/// <summary>
/// На которого подписан
/// </summary>
public record SubscriptionWithFollowingProfileDto
{
    public int SubscriptionId { get; init; }
    public int FollowingProfileId {get; init; }
    public string? FollowingFullName { get; init; } 
    public string? FollowingAvatarUrl { get; init; }
    public bool IsPaused { get; init; }
    public bool IsActive { get; init; }
    public DateTime SubscribedAt { get; init; }
}