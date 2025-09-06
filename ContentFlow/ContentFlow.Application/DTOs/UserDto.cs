namespace ContentFlow.Application.DTOs;

public record UserDto (
    int Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? AvatarUrl,
    DateTime CreatedAt);