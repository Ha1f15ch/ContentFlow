namespace ContentFlow.Application.DTOs;

public record TwoFactorCodeDto(
    int Id,
    int UserId,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    int AttemptCount,
    int MaxAttempts,
    bool IsUsed,
    string Purpose,
    DateTime? NextResendAt);