namespace ContentFlow.Application.DTOs;

public record UserDto(
    int Id,
    string Email,
    string UserName,
    string? AvatarUrl,
    DateTime CreatedAt,
    bool EmailConfirmed)
{
    private UserDto() : this(default, "", "", null, default, false) { }
}