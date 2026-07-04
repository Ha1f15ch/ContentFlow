using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Moderation.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Moderation;
using ContentFlow.Application.Interfaces.ModerationCase;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Moderation.Handlers;

public class ResolveModerationCaseCommandHandler : IRequestHandler<ResolveModerationCaseCommand, Unit>
{
    private readonly IModerationCaseRepository _moderationCaseRepository;
    private readonly IModerationService _moderationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResolveModerationCaseCommandHandler> _logger;

    public ResolveModerationCaseCommandHandler(
        IModerationCaseRepository moderationCaseRepository,
        IModerationService moderationService,
        IUnitOfWork unitOfWork,
        ILogger<ResolveModerationCaseCommandHandler> logger)
    {
        _moderationCaseRepository = moderationCaseRepository;
        _moderationService = moderationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(ResolveModerationCaseCommand request, CancellationToken cancellationToken)
    {
        await _moderationService.EnsureModeratorAsync(request.ModeratorId, cancellationToken);

        var moderationCase = await _moderationCaseRepository.GetByIdAsync(request.CaseId, cancellationToken);
        if (moderationCase == null)
            throw new NotFoundException("Moderation case not found");

        await _moderationService.ApplyDecisionAsync(
            moderationCase,
            request.Decision,
            request.ModeratorId,
            request.Note,
            cancellationToken);

        moderationCase.Resolve(request.Decision, request.ModeratorId, request.Note);
        await _moderationCaseRepository.UpdateAsync(moderationCase, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Moderation case {CaseId} resolved with decision {Decision} by moderator {ModeratorId}",
            request.CaseId,
            request.Decision,
            request.ModeratorId);

        return Unit.Value;
    }
}
