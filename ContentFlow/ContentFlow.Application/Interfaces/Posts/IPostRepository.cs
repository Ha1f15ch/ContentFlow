using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Interfaces.Posts;

public interface IPostRepository
{
    Task<Post> GetByIdAsync(int id, CancellationToken ct);
    Task<PaginatedResult<PostReadModel>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<List<Post>> GetPublishedAsync(CancellationToken ct);
    Task<List<Post>> GetByAuthorIdAsync(int authorId, CancellationToken ct);
    Task AddAsync(Post post, CancellationToken ct);
    Task UpdateAsync(Post post, CancellationToken ct);
    Task DeleteAsync(Post post, CancellationToken ct);
}