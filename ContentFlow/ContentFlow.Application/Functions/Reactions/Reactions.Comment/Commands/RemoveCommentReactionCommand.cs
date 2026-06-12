using ContentFlow.Application.DTOs.ReactionDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Comment.Commands;

public record RemoveCommentReactionCommand(
    int CommentId,
    int UserId
) : IRequest<ReactionResultDto>;