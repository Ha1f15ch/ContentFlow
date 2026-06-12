using ContentFlow.Application.Interfaces.PostReaction;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class PostReactionRepository : IPostReactionRepository
{
    private readonly ApplicationDbContext _context;
    
    public PostReactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<PostReaction?> GetByPostAndUserAsync(int postId, int userId, CancellationToken ct)
    {
        return await _context.PostReactions.FirstOrDefaultAsync(a => a.PostId == postId && a.UserId == userId, ct);
    }

    public async Task AddAsync(PostReaction postReaction, CancellationToken ct)
    {
        await _context.PostReactions.AddAsync(postReaction, ct);
    }

    public Task UpdateAsync(PostReaction postReaction, CancellationToken ct)
    {
        _context.PostReactions.Update(postReaction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PostReaction postReaction, CancellationToken ct)
    {
        _context.PostReactions.Remove(postReaction);
        return Task.CompletedTask;
    }

    public async Task<int> GetCountByReactionTypeAsync(int postId, ReactionType reactionType, CancellationToken ct)
    {
        return await _context.PostReactions.CountAsync(a => a.PostId == postId && a.ReactionType == reactionType, ct);
    }
}