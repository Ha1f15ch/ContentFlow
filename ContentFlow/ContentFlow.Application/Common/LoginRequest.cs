namespace ContentFlow.Application.Common;

public record LoginRequest(
    string Email,
    string Password);