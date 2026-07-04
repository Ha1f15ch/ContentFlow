using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.Moderation;

public interface IModerationService
{
    Task EnsureModeratorAsync(int userId, CancellationToken ct);

    Task ApplyDecisionAsync(
        Domain.Entities.ModerationCase moderationCase,
        ModerationDecision decision,
        int moderatorId,
        string? note,
        CancellationToken ct);

    Task RestoreContentIfHiddenAsync(Domain.Entities.ModerationCase moderationCase, CancellationToken ct);
}
