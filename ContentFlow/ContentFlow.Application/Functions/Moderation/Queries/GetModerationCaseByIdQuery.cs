using ContentFlow.Application.DTOs.ModerationDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Moderation.Queries;

public record GetModerationCaseByIdQuery(int CaseId, int RequesterId) : IRequest<ModerationCaseDetailsDto>;
