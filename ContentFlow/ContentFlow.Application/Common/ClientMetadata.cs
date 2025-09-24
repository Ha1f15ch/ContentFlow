namespace ContentFlow.Application.Common;

public record ClientMetadata(
    string? IpAddress,
    string? UserAgent,
    string? DeviceId,
    string? Location);