using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.Posts;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id, CancellationToken ct);
    Task<PaginatedResult<PostReadModel>> GetAllAsync(
        int page, 
        int pageSize, 
        PostFilter? filter,
        int? currentUserId = null,
        CancellationToken ct = default);
    Task<List<Post>> GetPublishedAsync(CancellationToken ct);
    Task<List<Post>> GetByAuthorIdAsync(int authorId, CancellationToken ct);
    Task AddAsync(Post post, CancellationToken ct);
    Task UpdateAsync(Post post, CancellationToken ct);
    Task DeleteAsync(Post post, CancellationToken ct);
}