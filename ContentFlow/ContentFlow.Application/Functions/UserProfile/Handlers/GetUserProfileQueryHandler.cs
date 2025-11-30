using AutoMapper;
using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Application.Functions.UserProfile.Queries;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.UserProfile.Handlers;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching profile for user ID: {UserId}", request.UserId);

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    
        if (profile == null)
        {
            _logger.LogWarning("UserProfile not found for user ID: {UserId}. Try to create new ...", request.UserId);
            
            profile = new Domain.Entities.UserProfile(request.UserId);
            profile = await  _userProfileRepository.CreateAsync(profile, cancellationToken);
        }

        var dto = new UserProfileDto(
            Id: profile.Id,
            UserId: profile.UserId,
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
            IsDeleted: profile.IsDeleted
        );

        _logger.LogDebug("UserProfile DTO created for user ID: {UserId}", request.UserId);
        return dto;
    }
}