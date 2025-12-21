using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Application.Functions.UserProfile.Commands;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.UserProfile.Handlers;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;
    private readonly IUserService _userService;
    
    public UpdateUserProfileCommandHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<UpdateUserProfileCommandHandler> logger,
        IUserService userService)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
        _userService = userService;
    }

    public async Task<UserProfileDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating profile for user ID: {UserId}", request.UserId);
        
        if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
        {
            _logger.LogWarning("Invalid gender value '{Gender}' provided for user ID: {UserId}", request.Gender, request.UserId);
            throw new ArgumentException($"Invalid gender value: {request.Gender}");
        }

        
        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (profile == null)
        {
            _logger.LogInformation("UserProfile not found for user ID: {UserId}. Creating a new one.", request.UserId);
            throw new  NullReferenceException($"UserProfile not found for user ID: {request.UserId}. Cannot be null.");
        }
        else
        {
            profile.UpdateProfile(
                firstName: request.FirstName,
                lastName: request.LastName,
                middleName: request.MiddleName,
                birthDate: request.BirthDate,
                city: request.City,
                bio: request.Bio,
                gender: gender);
        }
        
        profile = await _userProfileRepository.UpdateAsync(profile, cancellationToken);
        var user = await _userService.GetByIdAsync(profile.UserId, cancellationToken);
        
        var dto = new UserProfileDto(
            Id: profile.Id,
            UserId: profile.UserId,
            UserName: user.UserName,
            FirstName: profile.FirstName,
            LastName: profile.LastName,
            MiddleName: profile.MiddleName,
            BirthDate: profile.BirthDate,
            Age: profile.CalculateAgeByBirthDate(),
            City: profile.City,
            Bio: profile.Bio,
            AvatarUrl: profile.AvatarUrl,
            Gender: profile.Gender.ToString(),
            CreatedAt: profile.CreatedAt,
            UpdatedAt: profile.UpdatedAt,
            IsDeleted: profile.IsDeleted,
            null
        );

        _logger.LogInformation("Profile successfully updated for user ID: {UserId}", request.UserId);
        return dto;
    }
}