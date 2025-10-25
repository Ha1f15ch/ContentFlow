using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Commands;

public record UpdateCommentCommand(
    int CommentId, 
    int PostId,
    string NewCommentText, 
    int AuthorId) : IRequest<Unit>;
