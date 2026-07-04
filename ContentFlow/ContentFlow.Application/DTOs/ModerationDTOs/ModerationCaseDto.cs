using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ModerationDTOs;

public record ModerationCaseDto(
    int Id,
    int? PostId,
    int? CommentId,
    ModerationCaseStatus Status,
    ModerationPriority Priority,
    int ReportCount,
    int UniqueReporterCount,
    DateTime FirstReportedAt,
    DateTime LastReportedAt,
    int? AssignedModeratorId,
    DateTime CreatedAt);
