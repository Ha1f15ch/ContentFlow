namespace ContentFlow.Application.Common;

public record AuthResult(
    bool Success,
    string? Token = null,
    string? RefreshToken = null,
    string? Errors = null,
    bool RequiresEmailConfirmation = false,
    bool EmailSent = false,
    bool EmailAlreadyConfirmed = false,
    bool AccountDeleted = false,
    string? Message = null,
    int? RetryAfterSeconds = null);