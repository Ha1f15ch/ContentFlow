using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Post.Commands;

public record SetPostReactionCommand(
    int PostId,
    int UserId,
    ReactionType ReactionType
) : IRequest<ReactionResultDto>;