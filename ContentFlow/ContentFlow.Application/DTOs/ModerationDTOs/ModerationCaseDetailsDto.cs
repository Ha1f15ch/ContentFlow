using ContentFlow.Application.DTOs.ReportDTOs;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ModerationDTOs;

public record ModerationCaseDetailsDto(
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
    DateTime? ResolvedAt,
    int? ResolvedById,
    ModerationDecision? Decision,
    string? ModeratorNote,
    DateTime CreatedAt,
    IReadOnlyList<ReportDto> Reports);
