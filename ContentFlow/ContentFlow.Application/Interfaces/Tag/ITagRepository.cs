using ContentFlow.Application.Common;

namespace ContentFlow.Application.Interfaces.Tag;

public interface ITagRepository
{
    Task<Domain.Entities.Tag?> GetByIdAsync(int id, CancellationToken ct);
    Task<Domain.Entities.Tag?> GetByNameAsync(string name, CancellationToken ct);
    Task<Domain.Entities.Tag?> GetBySlugAsync(string slug, CancellationToken ct);
    Task<PaginatedResult<Domain.Entities.Tag>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task AddAsync(Domain.Entities.Tag tag, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.Tag tag, CancellationToken ct);
    Task DeleteAsync(Domain.Entities.Tag tag, CancellationToken ct);
}