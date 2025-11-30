using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Application.Functions.UserProfile.Commands;
using ContentFlow.Application.Interfaces.FileStorage;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.UserProfile.Handlers;

public class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<UpdateUserAvatarCommandHandler> _logger;
    private readonly IFileStorageService _fileStorageService;

    public UpdateUserAvatarCommandHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<UpdateUserAvatarCommandHandler> logger,
        IFileStorageService fileStorageService)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
        _fileStorageService = fileStorageService;
    }

    public async Task<UserProfileDto> Handle(UpdateUserAvatarCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateUserAvatar processing ...");
        
        var profile  = await _userProfileRepository.GetByUserIdAsync(command.UserId, cancellationToken);

        if (profile == null)
        {
            _logger.LogError($"No profile found for user id {command.UserId}");
            throw new NotFoundException("Profile not found");
        }

        if (!string.IsNullOrEmpty(profile.AvatarUrl))
        {
            _logger.LogWarning("Avatar url already exists. Remove old avatar.");
            await _fileStorageService.DeleteAvatarAsync(profile.AvatarUrl, cancellationToken);
        }
        
        _logger.LogInformation("Upload new avatar ...");
        var newAvatarUri = await _fileStorageService.UploadAvatarAsync(command.UserId, command.AvatarFile, cancellationToken);
        
        _logger.LogInformation("Upload new avatar complete.");
        
        profile.UpdateUserAvatarUri(newAvatarUri);
        var result = await _userProfileRepository.UpdateAsync(profile, cancellationToken);
        
        return new UserProfileDto(
            result.Id, 
            result.UserId, 
            result.FirstName, 
            result.LastName, 
            result.MiddleName, 
            result.BirthDate, 
            result.CalculateAgeByBirthDate(), 
            result.City, 
            result.Bio, 
            result.AvatarUrl, 
            result.Gender.ToString(), 
            result.CreatedAt, 
            result.UpdatedAt, 
            result.IsDeleted);
    }
}