using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record EnableNotificationsCommand(int FollowerUserId, int FollowingUserId) : IRequest<Unit>;