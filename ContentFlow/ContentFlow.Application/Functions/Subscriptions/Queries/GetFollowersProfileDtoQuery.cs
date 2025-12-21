using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Queries;

public record GetFollowersProfileDtoQuery(int UserId) : IRequest<List<SubscriptionWithFollowingProfileDto>>;