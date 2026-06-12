using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Comment.Commands;

public record SetCommentReactionCommand(
    int CommentId,
    int UserId,
    ReactionType ReactionType) : IRequest<ReactionResultDto>;