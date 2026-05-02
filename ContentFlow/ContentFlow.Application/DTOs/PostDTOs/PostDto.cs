using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs;

public record PostDto(
    int Id,
    string Title,
    string Slug,
    string Excerpt,
    string? Content,
    int AuthorId,
    int? AuthorProfileId,
    string AuthorName,
    string? AuthorAvatar,
    PostStatus Status,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    List<TagDto> Tags,
    int CommentCount);