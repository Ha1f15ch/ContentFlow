using System.Globalization;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Specifications;
using ContentFlow.Application.Specifications.Posts;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Post?> GetByIdAsync(int id, CancellationToken ct)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        if (post == null)
        {
            Console.WriteLine($"Post with ID {id} not found.");
        }
        
        return post;
    }

    public async Task<PaginatedResult<PostReadModel>> GetAllAsync(
        int page,
        int pageSize,
        PostFilter? filter,
        int? currentUserId = null,
        CancellationToken ct = default)
    {
        var baseQuery = _context.Posts
            .AsNoTracking()
            .Apply(
                new PostVisibilitySpecification(currentUserId),
                new PostFilterSpecification(filter),
                new PostSortingSpecification(filter?.Sort)
            );

        var totalCount = await baseQuery.CountAsync(ct);
        
        var posts = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var postIds = posts.Select(p => p.Id).ToList();
        var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();

        var authors = await _context.Users
            .AsNoTracking()
            .Where(u => authorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, ct);

        var commentCounts = await _context.Comments
            .AsNoTracking()
            .Where(c => postIds.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count, ct);

        var reactionCounts = await _context.PostReactions
            .AsNoTracking()
            .Where(r => postIds.Contains(r.PostId))
            .GroupBy(r => new { r.PostId, r.ReactionType })
            .Select(g => new { g.Key.PostId, g.Key.ReactionType, Count = g.Count() })
            .ToListAsync(ct);

        var currentUserReactions = currentUserId.HasValue
            ? await _context.PostReactions
                .AsNoTracking()
                .Where(r => postIds.Contains(r.PostId) && r.UserId == currentUserId.Value)
                .ToDictionaryAsync(r => r.PostId, r => (ReactionType?)r.ReactionType, ct)
            : new Dictionary<int, ReactionType?>();

        var postReadModels = posts.Select(post =>
        {
            authors.TryGetValue(post.AuthorId, out var author);
            var likesCount = reactionCounts
                .Where(r => r.PostId == post.Id && r.ReactionType == ReactionType.Like)
                .Sum(r => r.Count);
            var dislikesCount = reactionCounts
                .Where(r => r.PostId == post.Id && r.ReactionType == ReactionType.Dislike)
                .Sum(r => r.Count);

            return new PostReadModel(
                post.Id,
                post.Title,
                post.Slug,
                post.Excerpt,
                post.Content,
                post.AuthorId,
                post.Status,
                post.CreatedAt,
                post.PublishedAt,
                commentCounts.GetValueOrDefault(post.Id, 0),
                author == null ? "Unknown Author" : (author.UserName ?? "Unknown Author"),
                author?.AuthorAvatar,
                likesCount,
                dislikesCount,
                currentUserReactions.GetValueOrDefault(post.Id));
        }).ToList();

        return new PaginatedResult<PostReadModel>(
            postReadModels,
            totalCount,
            page,
            pageSize);
    }

    public async Task<List<Post>> GetPublishedAsync(CancellationToken ct)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(p => p.Status == PostStatus.Published)
            .ToListAsync(ct);
    }

    public async Task<List<Post>> GetByAuthorIdAsync(int authorId, CancellationToken ct)
    {
        return await _context.Posts
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Post post, CancellationToken ct)
    {
        await _context.Posts.AddAsync(post, ct);
    }

    public Task UpdateAsync(Post post, CancellationToken ct)
    {
        _context.Posts.Update(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Post post, CancellationToken ct)
    {
        post.MarkAsDeleted();
        _context.Posts.Update(post);
        return Task.CompletedTask;
    }
}