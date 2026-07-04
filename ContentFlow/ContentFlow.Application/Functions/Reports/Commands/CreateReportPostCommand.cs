using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Reports.Commands;

public record CreateReportPostCommand(
    int ReporterId,
    int PostId,
    ReportReasonType ReasonType,
    string? Description) : IRequest<int>;
