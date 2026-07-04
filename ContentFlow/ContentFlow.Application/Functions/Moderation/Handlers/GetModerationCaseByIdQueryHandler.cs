using ContentFlow.Application.DTOs.ModerationDTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Moderation.Queries;
using ContentFlow.Application.Interfaces.Moderation;
using ContentFlow.Application.Interfaces.ModerationCase;
using ContentFlow.Application.Interfaces.Report;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Moderation.Handlers;

public class GetModerationCaseByIdQueryHandler : IRequestHandler<GetModerationCaseByIdQuery, ModerationCaseDetailsDto>
{
    private readonly IModerationCaseRepository _moderationCaseRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IModerationService _moderationService;
    private readonly ILogger<GetModerationCaseByIdQueryHandler> _logger;

    public GetModerationCaseByIdQueryHandler(
        IModerationCaseRepository moderationCaseRepository,
        IReportRepository reportRepository,
        IModerationService moderationService,
        ILogger<GetModerationCaseByIdQueryHandler> logger)
    {
        _moderationCaseRepository = moderationCaseRepository;
        _reportRepository = reportRepository;
        _moderationService = moderationService;
        _logger = logger;
    }

    public async Task<ModerationCaseDetailsDto> Handle(
        GetModerationCaseByIdQuery request,
        CancellationToken cancellationToken)
    {
        await _moderationService.EnsureModeratorAsync(request.RequesterId, cancellationToken);

        var moderationCase = await _moderationCaseRepository.GetByIdAsync(request.CaseId, cancellationToken);
        if (moderationCase == null)
            throw new NotFoundException("Moderation case not found");

        var reports = moderationCase.IsForPost
            ? await _reportRepository.GetByPostIdAsync(moderationCase.PostId!.Value, cancellationToken)
            : await _reportRepository.GetByCommentIdAsync(moderationCase.CommentId!.Value, cancellationToken);

        _logger.LogDebug(
            "Moderation case {CaseId} loaded with {ReportCount} reports for requester {RequesterId}",
            request.CaseId,
            reports.Count,
            request.RequesterId);

        return ModerationMappings.ToDetailsDto(moderationCase, reports);
    }
}
