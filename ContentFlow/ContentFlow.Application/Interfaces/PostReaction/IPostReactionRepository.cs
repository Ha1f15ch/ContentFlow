using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.PostReaction;

public interface IPostReactionRepository
{
    Task<Domain.Entities.PostReaction?> GetByPostAndUserAsync(int postId, int userId, CancellationToken ct);
    Task AddAsync(Domain.Entities.PostReaction postReaction, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.PostReaction postReaction, CancellationToken ct);
    Task DeleteAsync(Domain.Entities.PostReaction postReaction, CancellationToken ct);
    Task<int> GetCountByReactionTypeAsync(int postId, ReactionType reactionType, CancellationToken ct);
}