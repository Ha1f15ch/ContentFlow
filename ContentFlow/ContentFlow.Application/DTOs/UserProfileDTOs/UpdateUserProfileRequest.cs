namespace ContentFlow.Application.DTOs.UserProfileDTOs;

public class UpdateUserProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? Gender { get; set; }
}