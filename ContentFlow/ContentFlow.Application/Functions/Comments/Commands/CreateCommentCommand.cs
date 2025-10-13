using MediatR;

namespace ContentFlow.Application.Functions.Comments.Commands;

public record CreateCommentCommand(
    int PostId,
    int AuthorId,
    string Content,
    int? ParentCommentId = null) : IRequest<int>;