using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record UnsubscribeCommand(int FollowerUserId, int FollowingUserId) : IRequest<Unit>;