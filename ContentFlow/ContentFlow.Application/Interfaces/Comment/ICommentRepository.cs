using ContentFlow.Application.Common;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Interfaces.Comment;

public interface ICommentRepository
{
    Task<Domain.Entities.Comment> GetByIdAsync(int id, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Comment>> GetByPostIdAsync(int postId, int page, int pageSize, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Comment>> GetByAuthorIdAsync(int authorId, int page, int pageSize, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Comment>> GetApprovedAsync(int page, int pageSize, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Comment>> GetPendingAsync(int page, int pageSize, CancellationToken ct);
    Task<List<Domain.Entities.Comment>> GetApprovedByPostIdAsync(int postId, CancellationToken ct);
    Task<int> GetCountAsync(int postId, CancellationToken ct);
    Task AddAsync(Domain.Entities.Comment comment, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Comment comment, CancellationToken ct);
    Task DeleteAsync(Domain.Entities.Comment comment, CancellationToken ct);
}