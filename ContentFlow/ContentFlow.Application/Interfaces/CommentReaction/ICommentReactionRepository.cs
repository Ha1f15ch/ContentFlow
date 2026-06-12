using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.CommentReaction;

public interface ICommentReactionRepository
{
    Task<Domain.Entities.CommentReaction?> GetByCommentAndUserAsync(int commentId, int userId, CancellationToken ct);
    Task AddAsync(Domain.Entities.CommentReaction commentReaction, CancellationToken ct);
    Task UpdateAsync(Domain.Entities.CommentReaction commentReaction, CancellationToken ct);
    Task DeleteAsync(Domain.Entities.CommentReaction commentReaction, CancellationToken ct);
    Task<int> GetCountByReactionTypeAsync(int commentId, ReactionType reactionType, CancellationToken ct);
}