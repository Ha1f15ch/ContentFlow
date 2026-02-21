using MediatR;

namespace ContentFlow.Application.Functions.Posts.Commands;

public record CreatePostCommand (
    string Title,
    string Content,
    int AuthorId
    ) : IRequest<int>;