namespace ContentFlow.Application.DTOs;

public record RefreshTokenDto(
    int Id,
    int UserId,
    string TokenHash,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    string CreatedByIp,
    bool IsRevoked,
    string? DeviceId);