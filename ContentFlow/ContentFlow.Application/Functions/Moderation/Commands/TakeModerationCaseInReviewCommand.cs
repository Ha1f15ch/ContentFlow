using MediatR;

namespace ContentFlow.Application.Functions.Moderation.Commands;

public record TakeModerationCaseInReviewCommand(int CaseId, int ModeratorId) : IRequest<Unit>;
