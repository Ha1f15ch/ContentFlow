using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record PauseSubscriptionCommand(int FollowerUserId, int FollowingUserId) : IRequest<Unit>;