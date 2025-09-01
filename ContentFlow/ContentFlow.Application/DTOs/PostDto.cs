using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs;

public record PostDto(
    int Id,
    string Title,
    string Slug,
    string Excerpt,
    int AuthorId,
    string AuthorName,
    string? AuthorAvatar,
    PostStatus Status,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    List<TagDto> Tags,
    int CommentCount);