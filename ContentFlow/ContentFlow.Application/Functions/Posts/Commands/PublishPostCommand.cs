using MediatR;

namespace ContentFlow.Application.Functions.Posts.Commands;

public record PublishPostCommand(
    int PostId,
    int UserId) : IRequest<bool>;