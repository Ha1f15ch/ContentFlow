using ContentFlow.Application.Functions.Reports.Commands;
using ContentFlow.Application.Interfaces.Report;
using MediatR;

namespace ContentFlow.Application.Functions.Reports.Handlers;

public class CreateReportCommentCommandHandler : IRequestHandler<CreateReportCommentCommand, int>
{
    private readonly IReportSubmissionService _reportSubmissionService;

    public CreateReportCommentCommandHandler(IReportSubmissionService reportSubmissionService)
    {
        _reportSubmissionService = reportSubmissionService;
    }

    public Task<int> Handle(CreateReportCommentCommand request, CancellationToken cancellationToken) =>
        _reportSubmissionService.SubmitForCommentAsync(
            request.ReporterId,
            request.CommentId,
            request.ReasonType,
            request.Description,
            cancellationToken);
}
