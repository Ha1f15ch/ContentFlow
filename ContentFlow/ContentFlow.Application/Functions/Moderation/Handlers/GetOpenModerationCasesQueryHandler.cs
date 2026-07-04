using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs.ModerationDTOs;
using ContentFlow.Application.Functions.Moderation.Queries;
using ContentFlow.Application.Interfaces.Moderation;
using ContentFlow.Application.Interfaces.ModerationCase;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Moderation.Handlers;

public class GetOpenModerationCasesQueryHandler
    : IRequestHandler<GetOpenModerationCasesQuery, PaginatedResult<ModerationCaseDto>>
{
    private readonly IModerationCaseRepository _moderationCaseRepository;
    private readonly IModerationService _moderationService;
    private readonly ILogger<GetOpenModerationCasesQueryHandler> _logger;

    public GetOpenModerationCasesQueryHandler(
        IModerationCaseRepository moderationCaseRepository,
        IModerationService moderationService,
        ILogger<GetOpenModerationCasesQueryHandler> logger)
    {
        _moderationCaseRepository = moderationCaseRepository;
        _moderationService = moderationService;
        _logger = logger;
    }

    public async Task<PaginatedResult<ModerationCaseDto>> Handle(
        GetOpenModerationCasesQuery request,
        CancellationToken cancellationToken)
    {
        await _moderationService.EnsureModeratorAsync(request.RequesterId, cancellationToken);

        if (request.Page <= 0)
            throw new ArgumentException("Page must be greater than zero.", nameof(request.Page));

        if (request.PageSize <= 0)
            throw new ArgumentException("PageSize must be greater than zero.", nameof(request.PageSize));

        var result = await _moderationCaseRepository.GetOpenCasesAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        _logger.LogDebug(
            "Retrieved {ItemCount} open moderation cases out of {TotalCount}",
            result.Items.Count,
            result.TotalCount);

        var items = result.Items.Select(ModerationMappings.ToDto).ToList();
        return new PaginatedResult<ModerationCaseDto>(items, result.TotalCount, result.Page, result.PageSize);
    }
}
