using MediatR;

namespace ContentFlow.Application.Functions.Posts.Commands;

public record UpdatePostCommand(
    int PostId,
    string Title,
    string Content,
    int? CategoryId,
    List<int> TagIds,
    int AuthorId) : IRequest;