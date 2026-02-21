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
        
        var pagedPosts = baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var posts = await (
            from post in pagedPosts
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
                (author.FirstName + " " + author.LastName).Trim(),
                author.AuthorAvatar
            )
        ).ToListAsync(ct);

        return new PaginatedResult<PostReadModel>(
            posts,
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