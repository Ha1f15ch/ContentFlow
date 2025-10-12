using ContentFlow.Application.Common;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Comment> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Comment with id {id} not found");
    }

    public async Task<int> GetCountAsync(int postId, CancellationToken ct)
    {
        return await _context.Comments.CountAsync(c => c.PostId == postId, ct);
    }

    public async Task<PaginatedResult<Comment>> GetByPostIdAsync(int postId, int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Comments
            .CountAsync(c => c.PostId == postId && c.Status == CommentStatus.Approved, ct);

        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId && c.Status == CommentStatus.Approved)
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<Comment>(comments, totalCount, page, pageSize);
    }

    public async Task<PaginatedResult<Comment>> GetByAuthorIdAsync(int authorId, int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Comments
            .CountAsync(c => c.AuthorId == authorId && c.Status == CommentStatus.Approved, ct);
        
        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.AuthorId == authorId && c.Status == CommentStatus.Approved)
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedResult<Comment>(comments, totalCount, page, pageSize);
    }

    public async Task<PaginatedResult<Comment>> GetApprovedAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Comments
            .CountAsync(c => c.Status == CommentStatus.Approved, ct);
        
        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.Status == CommentStatus.Approved)
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedResult<Comment>(comments, totalCount, page, pageSize);
    }

    public async Task<PaginatedResult<Comment>> GetPendingAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Comments
            .CountAsync(c => c.Status == CommentStatus.Pending, ct);
        
        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.Status == CommentStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedResult<Comment>(comments, totalCount, page, pageSize);
    }

    public async Task<List<Comment>> GetApprovedByPostIdAsync(int postId, CancellationToken ct)
    {
        if (postId == 0)
        {
            return new List<Comment>();
        }
        
        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.PostId == postId &&
                        c.Status == CommentStatus.Approved &&
                        !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Comment comment, CancellationToken ct)
    {
        await _context.Comments.AddAsync(comment, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Comment comment, CancellationToken ct)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Comment comment, CancellationToken ct)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync(ct);
    }
}