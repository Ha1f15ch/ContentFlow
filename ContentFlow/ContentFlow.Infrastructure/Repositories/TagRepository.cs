using ContentFlow.Application.Common;
using ContentFlow.Application.Interfaces.Tag;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Tag?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Tags
                   .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        var normalizedName = name.Trim().ToLower();
        
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == normalizedName, ct);
    }

    public async Task<Tag?> GetBySlugAsync(string slug, CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be null or empty", nameof(slug));
        
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Slug == slug, ct);
    }

    public async Task<PaginatedResult<Tag>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Tags
            .CountAsync(ct);

        var tags = await _context.Tags
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<Tag>(tags, totalCount, page, pageSize);
    }

    public async Task AddAsync(Tag tag, CancellationToken ct)
    {
        await _context.Tags.AddAsync(tag, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tag tag, CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Tag tag, CancellationToken ct)
    {
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(ct);
    }
}