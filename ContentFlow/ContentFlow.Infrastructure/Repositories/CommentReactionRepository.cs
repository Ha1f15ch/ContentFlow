using ContentFlow.Application.Interfaces.CommentReaction;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class CommentReactionRepository : ICommentReactionRepository
{
    private readonly ApplicationDbContext _context;
    
    public CommentReactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<CommentReaction?> GetByCommentAndUserAsync(int commentId, int userId, CancellationToken ct)
    {
        return await _context.CommentReactions.FirstOrDefaultAsync(a => a.CommentId == commentId && a.UserId == userId, ct);
    }

    public async Task AddAsync(CommentReaction commentReaction, CancellationToken ct)
    {
        await _context.CommentReactions.AddAsync(commentReaction, ct);
    }

    public Task UpdateAsync(CommentReaction commentReaction, CancellationToken ct)
    {
        _context.CommentReactions.Update(commentReaction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(CommentReaction commentReaction, CancellationToken ct)
    {
        _context.CommentReactions.Remove(commentReaction);
        return Task.CompletedTask;
    }

    public async Task<int> GetCountByReactionTypeAsync(int commentId, ReactionType reactionType, CancellationToken ct)
    {
        return await  _context.CommentReactions.CountAsync(a => a.CommentId == commentId && a.ReactionType == reactionType, ct);
    }
}