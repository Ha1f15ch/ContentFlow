using MediatR;

namespace ContentFlow.Application.Functions.Comments.Commands;

public record DeleteCommentCommand(
    int CommentId, 
    int PostId, 
    int AuthorId) : IRequest<bool>;