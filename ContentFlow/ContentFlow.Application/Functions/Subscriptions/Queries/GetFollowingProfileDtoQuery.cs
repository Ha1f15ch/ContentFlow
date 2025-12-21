using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Queries;

public record GetFollowingProfileDtoQuery(int UserId) : IRequest<List<SubscriptionWithFollowerProfileDto>>;