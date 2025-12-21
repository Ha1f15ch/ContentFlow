using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record SubscribeCommand(int FollowerUserId, int  FollowingUserId) : IRequest<Unit>;