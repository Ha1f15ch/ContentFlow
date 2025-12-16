using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using ContentFlow.Application.Functions.Subscriptions.Queries;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class GetFollowersProfileDtoQueryHandler : IRequestHandler<GetFollowersProfileDtoQuery, List<SubscriptionWithFollowingProfileDto>>
{
    private readonly ILogger<GetFollowersProfileDtoQueryHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetFollowersProfileDtoQueryHandler(
        ILogger<GetFollowersProfileDtoQueryHandler> logger,
        IUserProfileRepository userProfileRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<SubscriptionWithFollowingProfileDto>> Handle(GetFollowersProfileDtoQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Get GetFollowersProfileDtoQueryHandler.Handle() called.");
        
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (userProfile == null)
        {
            _logger.LogError($"Get GetFollowersProfileDtoQueryHandler.Handle() returned null.");
            throw new NullReferenceException("UserProfile not found by userId.");
        }
        
        _logger.LogInformation("Fetching user profiles - on who's follower user id - {UserId}", request.UserId);

        var listUserProfiles = await 
            _subscriptionRepository.GetListSubscriptionFollowersByFollowerAsync(request.UserId, cancellationToken);
        _logger.LogInformation("Get {CountSubscriptions} subscriptions.", listUserProfiles.Count());

        return listUserProfiles;
    }
}