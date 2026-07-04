using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ReportDTOs;

public record ReportDto(
    int Id,
    int ReporterId,
    int? PostId,
    int? CommentId,
    ReportReasonType ReasonType,
    string? Description,
    DateTime CreatedAt);
