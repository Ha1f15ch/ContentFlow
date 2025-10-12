namespace ContentFlow.Application.DTOs;

public record CommentDto(
    int Id,
    string Content,
    string AuthorName,
    DateTime CreatedAt,
    List<CommentDto> Comments,
    int? ParentCommentId);