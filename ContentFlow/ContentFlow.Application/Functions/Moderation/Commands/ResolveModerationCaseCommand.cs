using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Moderation.Commands;

public record ResolveModerationCaseCommand(
    int CaseId,
    int ModeratorId,
    ModerationDecision Decision,
    string? Note) : IRequest<Unit>;
