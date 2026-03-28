using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs;

public record CommentDto(
    int Id,
    int PostId,
    string Content,
    string UserName,
    DateTime CreatedAt,
    List<CommentDto> Comments,
    int? ParentCommentId,
    string CommentStatus,
    bool IsDeleted);