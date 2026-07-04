using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs.ModerationDTOs;
using ContentFlow.Application.DTOs.ReportDTOs;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Functions.Moderation;

internal static class ModerationMappings
{
    public static ModerationCaseDto ToDto(ModerationCase moderationCase) =>
        new(
            moderationCase.Id,
            moderationCase.PostId,
            moderationCase.CommentId,
            moderationCase.Status,
            moderationCase.Priority,
            moderationCase.ReportCount,
            moderationCase.UniqueReporterCount,
            moderationCase.FirstReportedAt,
            moderationCase.LastReportedAt,
            moderationCase.AssignedModeratorId,
            moderationCase.CreatedAt);

    public static ReportDto ToDto(Report report) =>
        new(
            report.Id,
            report.ReporterId,
            report.PostId,
            report.CommentId,
            report.ReasonType,
            report.Description,
            report.CreatedAt);

    public static ModerationCaseDetailsDto ToDetailsDto(
        ModerationCase moderationCase,
        IReadOnlyList<Report> reports) =>
        new(
            moderationCase.Id,
            moderationCase.PostId,
            moderationCase.CommentId,
            moderationCase.Status,
            moderationCase.Priority,
            moderationCase.ReportCount,
            moderationCase.UniqueReporterCount,
            moderationCase.FirstReportedAt,
            moderationCase.LastReportedAt,
            moderationCase.AssignedModeratorId,
            moderationCase.ResolvedAt,
            moderationCase.ResolvedById,
            moderationCase.Decision,
            moderationCase.ModeratorNote,
            moderationCase.CreatedAt,
            reports.Select(ToDto).ToList());
}
