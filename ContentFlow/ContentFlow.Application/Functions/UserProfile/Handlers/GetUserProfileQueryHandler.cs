using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs.SubscriptionDTOs;
using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Application.Functions.UserProfile.Queries;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.UserProfile.Handlers;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;
    private readonly IUserService _userService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    
    public GetUserProfileQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<GetUserProfileQueryHandler> logger,
        IUserService userService,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching profile for user ID: {UserId}", request.UserProfileId);
        var requesterUserProfile = await _userProfileRepository.GetByUserIdAsync(request.RequesterUserId, cancellationToken);

        if (requesterUserProfile == null)
        {
            _logger.LogError("Requester UserProfile not found for user ID: {UserId}", request.RequesterUserId);
            throw  new NotFoundException($"Requester UserProfile not found for user ID: {request.RequesterUserId}");
        }

        if (request.UserProfileId == requesterUserProfile.Id)
        {
            _logger.LogInformation("Fetching himself profile: {UserId}", request.RequesterUserId);
        }
        else
        {
             _logger.LogInformation("User {request.RequesterId} fetching other profile ID: {request.UserProfileId}",  request.RequesterUserId, request.UserProfileId);
             var isTrusted = await _userService.IsInRoleAsync(requesterUserProfile.UserId, RoleConstants.User) ||
                             await _userService.IsInRoleAsync(requesterUserProfile.UserId, RoleConstants.ContentEditor) ||
                             await _userService.IsInRoleAsync(requesterUserProfile.UserId, RoleConstants.Moderator) ||
                             await _userService.IsInRoleAsync(requesterUserProfile.UserId, RoleConstants.Admin);
             
             if (!isTrusted)
             {
                 _logger.LogWarning("Access denied: user {UserId} does not have permission to look other userprofile", request.RequesterUserId);
                 throw new UnauthorizedAccessException("You do not have permission for watch.");
             }
        }
        
        var profile = await _userProfileRepository.GetByIdAsync(request.UserProfileId, cancellationToken);
    
        if (profile == null)
        {
            _logger.LogWarning("Target UserProfile with ID: {UserId}. not found ", request.UserProfileId);
            throw new NotFoundException($"Target UserProfile with ID: {request.UserProfileId}");
        }
        
        var user = await _userService.GetByIdAsync(profile.UserId, cancellationToken);
        var subscriptionInfo = await _subscriptionRepository.GetByFollowerAndFollowingAsync(request.RequesterUserId, request.UserProfileId, cancellationToken);

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
            SubscriptionInfo: subscriptionInfo != null ? _mapper.Map<SubscriptionInfoDto>(subscriptionInfo) : null
        );

        _logger.LogDebug("UserProfile ID: {UserId} loaded", request.UserProfileId);
        return dto;
    }
}