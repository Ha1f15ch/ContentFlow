using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Interfaces.Report;

public interface IReportRepository
{
    Task<Domain.Entities.Report?> GetByIdAsync(int id, CancellationToken ct);
    Task<Domain.Entities.Report?> GetByReporterAndPostAsync(int reporterId, int postId, CancellationToken ct);
    Task<Domain.Entities.Report?> GetByReporterAndCommentAsync(int reporterId, int commentId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Report>> GetByPostIdAsync(int postId, CancellationToken ct);
    Task<IReadOnlyList<Domain.Entities.Report>> GetByCommentIdAsync(int commentId, CancellationToken ct);
    Task AddAsync(Domain.Entities.Report report, CancellationToken ct);
}
