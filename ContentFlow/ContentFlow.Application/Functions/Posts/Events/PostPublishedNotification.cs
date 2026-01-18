using MediatR;

namespace ContentFlow.Application.Functions.Posts.Events;

public record PostPublishedNotification(
    int PostId,
    int AuthorUserProfileId,
    DateTime PublishedAt
) : INotification;