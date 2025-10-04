using MediatR;

namespace ContentFlow.Application.Functions.Posts.Commands;

public record DeletePostCommand(
    int PostId,
    int UserInitiator) : IRequest;
