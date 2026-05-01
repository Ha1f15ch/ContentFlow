using MediatR;

namespace ContentFlow.Application.Functions.Subscriptions.Commands;

public record ResumeSubscriptionCommand(int FollowerUserId, int FollowingProfileId) : IRequest<Unit>;