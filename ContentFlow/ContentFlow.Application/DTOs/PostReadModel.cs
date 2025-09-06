using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs;

public record PostReadModel(
    int Id,
    string Title,
    string Slug,
    string Excerpt,
    int AuthorId,
    PostStatus Status,
    DateTime CreatedAt,
    DateTime PublishedAt,
    int CommentCount);