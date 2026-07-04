using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Reports.Commands;

public record CreateReportCommentCommand(
    int ReporterId,
    int CommentId,
    ReportReasonType ReasonType,
    string? Description) : IRequest<int>;
