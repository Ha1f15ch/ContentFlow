namespace ContentFlow.Application.DTOs;

public record UserDto (
    string Id,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime CreatedAt);