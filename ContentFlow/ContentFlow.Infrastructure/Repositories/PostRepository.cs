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

    public async Task<PaginatedResult<PostReadModel>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Posts
            .AsNoTracking()
            .Where(p => p.Status == PostStatus.Published);
        
        var totalCount = await query.CountAsync(ct);
        
        var posts = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .GroupJoin(
                _context.Comments,
                post => post.Id,
                comment => comment.PostId,
                (post, comments) => new PostReadModel(
                    post.Id,
                    post.Title,
                    post.Slug,
                    post.Excerpt,
                    post.AuthorId,
                    post.Status,
                    post.CreatedAt,
                    post.PublishedAt,
                    comments.Count()
                )
            )
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