using System.Globalization;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Domain.Exceptions;
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
        string? search = null,
        int? categoryId = null,
        PostStatus? status = null,
        int? currentUserId = null,
        CancellationToken ct = default)
    {
        var query = _context.Posts
            .AsNoTracking()
            .AsQueryable();

        if (currentUserId.HasValue)
        {
            var userId =  currentUserId.Value;
            query = query.Where(p => 
                p.Status == PostStatus.Published ||
                p.Status == PostStatus.Archived || 
                (p.Status == PostStatus.Draft && p.AuthorId == userId) ||
                (p.Status == PostStatus.PendingModeration && p.AuthorId == userId));
        }
        else
        {
            query = query.Where(p => p.Status == PostStatus.Published);
        }
        
        // Search
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
        }
        
        // Filter by category
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var postQuery = from post in query
            join author in _context.Users on post.AuthorId equals author.Id
            join comment in _context.Comments on post.Id equals comment.PostId into comments
            select new PostReadModel(
                post.Id,
                post.Title,
                post.Slug,
                post.Excerpt,
                post.AuthorId,
                post.Status,
                post.CreatedAt,
                post.PublishedAt,
                comments.Count(),
                $"{author.FirstName} {author.LastName}".Trim(),
                author.AuthorAvatar
                );
        
        var totalCount = await postQuery.CountAsync(ct);
        
        var posts = await postQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedResult<PostReadModel>(posts, totalCount, page, pageSize);
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
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Post post, CancellationToken ct)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(ct); 
    }

    public async Task DeleteAsync(Post post, CancellationToken ct)
    {
        post.MarkAsDeleted();
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(ct);
    }
}