using ContentFlow.Application.Common;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Interfaces.Category;

public interface ICategoryRepository
{
    Task<Domain.Entities.Category> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Domain.Entities.Category category, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Category category, CancellationToken ct);
    Task DeleteAsync(Domain.Entities.Category category, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Category>> GetPaginatedAsync(int page, int pageSize, CancellationToken ct);
}