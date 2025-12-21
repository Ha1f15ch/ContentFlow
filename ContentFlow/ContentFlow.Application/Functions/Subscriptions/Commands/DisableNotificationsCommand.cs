using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record DisableNotificationsCommand(int FollowerUserId, int FollowingUserId) : IRequest<Unit>;