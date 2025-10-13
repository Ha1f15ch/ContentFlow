namespace ContentFlow.Application.DTOs;

public record CommentDtoToCreate(string Content, int? ParentCommentId);