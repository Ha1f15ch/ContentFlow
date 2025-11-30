namespace ContentFlow.Application.DTOs.UserProfileDTOs;

public record UserProfileDto(
    int Id,
    int UserId,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    DateOnly? BirthDate,
    int? Age,
    string? City,
    string? Bio,
    string? AvatarUrl,
    string Gender,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsDeleted);