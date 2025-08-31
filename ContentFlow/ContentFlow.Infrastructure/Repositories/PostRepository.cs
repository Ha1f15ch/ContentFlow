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
    
    public async Task<Post> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException($"Post with ID {id} not found.");
    }

    public async Task<List<Post>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Posts
            .AsNoTracking()
            .ToListAsync(ct);
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
        _context.Posts.Update(post);
        await _context.SaveChangesAsync(ct);
    }
}