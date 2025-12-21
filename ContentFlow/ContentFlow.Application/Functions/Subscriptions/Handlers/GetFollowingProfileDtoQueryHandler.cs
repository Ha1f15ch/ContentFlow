using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using ContentFlow.Application.Functions.Subscriptions.Queries;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Subscriptions.Handlers;

public class GetFollowingProfileDtoQueryHandler : IRequestHandler<GetFollowingProfileDtoQuery, List<SubscriptionWithFollowerProfileDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetFollowingProfileDtoQueryHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public GetFollowingProfileDtoQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<GetFollowingProfileDtoQueryHandler> logger,
        ISubscriptionRepository subscriptionRepository)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
    }
/// <summary>
/// Список подписчиков пользователя
/// </summary>
/// <param name="request"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
/// <exception cref="NullReferenceException"></exception>
    public async Task<List<SubscriptionWithFollowerProfileDto>> Handle(GetFollowingProfileDtoQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Get GetFollowingProfileDtoQueryHandler.Handle() called.");
        
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (userProfile == null)
        {
            _logger.LogError($"Get GetFollowingProfileDtoQueryHandler.Handle() returned null.");
            throw new NullReferenceException("UserProfile not found by userId.");
        }
        
        _logger.LogInformation("Fetching user profiles - on who's following user {UserId}", request.UserId);

        var listUserProfiles = await 
            _subscriptionRepository.GetListSubscriptionFollowingAsync(request.UserId, cancellationToken);
        _logger.LogInformation("Get {CountSubscriptions} subscriptions.", listUserProfiles.Count());

        return listUserProfiles;
    }
}