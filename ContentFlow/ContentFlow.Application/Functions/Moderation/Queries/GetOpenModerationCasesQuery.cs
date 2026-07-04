using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs.ModerationDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Moderation.Queries;

public record GetOpenModerationCasesQuery(
    int RequesterId,
    int Page = 1,
    int PageSize = 10) : IRequest<PaginatedResult<ModerationCaseDto>>;
