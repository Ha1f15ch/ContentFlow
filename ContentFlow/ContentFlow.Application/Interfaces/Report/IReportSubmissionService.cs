using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.Report;

public interface IReportSubmissionService
{
    Task<int> SubmitForPostAsync(
        int reporterId,
        int postId,
        ReportReasonType reasonType,
        string? description,
        CancellationToken ct);

    Task<int> SubmitForCommentAsync(
        int reporterId,
        int commentId,
        ReportReasonType reasonType,
        string? description,
        CancellationToken ct);
}
