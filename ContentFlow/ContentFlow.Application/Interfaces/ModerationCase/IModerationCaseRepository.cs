using ContentFlow.Application.Common;

namespace ContentFlow.Application.Interfaces.ModerationCase;

public interface IModerationCaseRepository
{
    Task<Domain.Entities.ModerationCase?> GetByIdAsync(int id, CancellationToken ct);
    Task<Domain.Entities.ModerationCase?> GetOpenByPostIdAsync(int postId, CancellationToken ct);
    Task<Domain.Entities.ModerationCase?> GetOpenByCommentIdAsync(int commentId, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.ModerationCase>> GetOpenCasesAsync(int page, int pageSize, CancellationToken ct);
    Task AddAsync(Domain.Entities.ModerationCase moderationCase, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.ModerationCase moderationCase, CancellationToken ct);
}
