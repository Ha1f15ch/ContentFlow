using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.UserProfile.Commands;

public record UpdateUserProfileCommand(
    int UserId,
    string? FirstName,
    string? LastName,
    string? MiddleName,
    DateOnly? BirthDate,
    string? City,
    string? Bio,
    string? Gender) : IRequest<UserProfileDto>;