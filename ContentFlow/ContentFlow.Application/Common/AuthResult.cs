namespace ContentFlow.Application.Common;

public record AuthResult(
    bool Success,
    string? Token = null,
    string? RefreshToken = null,
    List<string>? Errors = null);