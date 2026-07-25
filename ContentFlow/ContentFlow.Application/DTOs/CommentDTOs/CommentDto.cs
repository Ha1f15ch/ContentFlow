using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs;

public record CommentDto(
    int Id,
    int PostId,
    string Content,
    string UserName,
    int? AuthorProfileId,
    DateTime CreatedAt,
    List<CommentDto> Comments,
    int? ParentCommentId,
    string CommentStatus,
    bool IsDeleted,
    int LikesCount,
    int DislikesCount,
    ReactionType? CurrentUserReaction);