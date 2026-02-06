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

public class GetMyUserProfileQueryHandler : IRequestHandler<GetMyUserProfileQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetMyUserProfileQueryHandler> _logger;
    private readonly IUserService _userService;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;

    public GetMyUserProfileQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<GetMyUserProfileQueryHandler> logger,
        IUserService userService,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
        _userService = userService;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetMyUserProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching profile for user ID: {UserId}", request.UserId);

        var user = await _userService.GetByIdAsync(request.UserId, cancellationToken);
        
        var isTrusted = await _userService.IsInRoleAsync(user.Id, RoleConstants.User) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.ContentEditor) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Moderator) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Admin);
             
        if (!isTrusted)
        {
            _logger.LogWarning("Access denied: user {UserId} does not have permission to look other userprofile", request.UserId);
            throw new UnauthorizedAccessException("You do not have permission for watch.");
        }
        
        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
    
        if (profile == null)
        {
            _logger.LogWarning("Target UserProfile with ID: {UserId}. not found ", request.UserId);
            throw new NotFoundException($"Target UserProfile with user ID: {request.UserId}");
        }

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
            SubscriptionInfo: null // TODO наверное стоит не загружать здесь кучу всего списками, а просто посчитать число подписчиков, число подписок и отдельными запросами получать списки
        );

        _logger.LogDebug("UserProfile ID: {UserId} loaded", request.UserId);
        return dto;
    }
}