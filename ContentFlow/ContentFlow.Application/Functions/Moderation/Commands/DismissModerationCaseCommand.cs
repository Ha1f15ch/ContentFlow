using MediatR;

namespace ContentFlow.Application.Functions.Moderation.Commands;

public record DismissModerationCaseCommand(
    int CaseId,
    int ModeratorId,
    string? Note) : IRequest<Unit>;
