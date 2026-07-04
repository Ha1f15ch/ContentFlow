using ContentFlow.Application.Functions.Reports.Commands;
using ContentFlow.Application.Interfaces.Report;
using MediatR;

namespace ContentFlow.Application.Functions.Reports.Handlers;

public class CreateReportPostCommandHandler : IRequestHandler<CreateReportPostCommand, int>
{
    private readonly IReportSubmissionService _reportSubmissionService;

    public CreateReportPostCommandHandler(IReportSubmissionService reportSubmissionService)
    {
        _reportSubmissionService = reportSubmissionService;
    }

    public Task<int> Handle(CreateReportPostCommand request, CancellationToken cancellationToken) =>
        _reportSubmissionService.SubmitForPostAsync(
            request.ReporterId,
            request.PostId,
            request.ReasonType,
            request.Description,
            cancellationToken);
}
