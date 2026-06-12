using ContentFlow.Application.DTOs.ReactionDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Post.Commands;

public record RemovePostReactionCommand(
    int PostId,
    int UserId
) : IRequest<ReactionResultDto>;