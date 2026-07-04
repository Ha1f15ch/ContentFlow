using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Moderation.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Moderation;
using ContentFlow.Application.Interfaces.ModerationCase;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Moderation.Handlers;

public class TakeModerationCaseInReviewCommandHandler : IRequestHandler<TakeModerationCaseInReviewCommand, Unit>
{
    private readonly IModerationCaseRepository _moderationCaseRepository;
    private readonly IModerationService _moderationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TakeModerationCaseInReviewCommandHandler> _logger;

    public TakeModerationCaseInReviewCommandHandler(
        IModerationCaseRepository moderationCaseRepository,
        IModerationService moderationService,
        IUnitOfWork unitOfWork,
        ILogger<TakeModerationCaseInReviewCommandHandler> logger)
    {
        _moderationCaseRepository = moderationCaseRepository;
        _moderationService = moderationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(TakeModerationCaseInReviewCommand request, CancellationToken cancellationToken)
    {
        await _moderationService.EnsureModeratorAsync(request.ModeratorId, cancellationToken);

        var moderationCase = await _moderationCaseRepository.GetByIdAsync(request.CaseId, cancellationToken);
        if (moderationCase == null)
            throw new NotFoundException("Moderation case not found");

        moderationCase.TakeInReview(request.ModeratorId);
        await _moderationCaseRepository.UpdateAsync(moderationCase, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Moderation case {CaseId} taken in review by moderator {ModeratorId}",
            request.CaseId,
            request.ModeratorId);

        return Unit.Value;
    }
}
