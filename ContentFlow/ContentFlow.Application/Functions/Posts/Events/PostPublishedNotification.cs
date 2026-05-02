using MediatR;

namespace ContentFlow.Application.Functions.Posts.Events;

public record PostPublishedNotification(
    int PostId,
    int AuthorProfileId,
    DateTime PublishedAt
) : INotification;