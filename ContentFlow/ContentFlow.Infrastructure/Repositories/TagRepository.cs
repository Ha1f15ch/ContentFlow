using ContentFlow.Application.Common;
using ContentFlow.Application.Interfaces.Tag;
using ContentFlow.Domain.Entities;
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
    
    public Task<Tag> GetByIdAsync(int id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Tag> GetByNameAsync(string name, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Tag> GetBySlugAsync(string slug, CancellationToken ct)
    {
        throw new NotImplementedException();
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

    public Task AddAsync(Tag tag, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Tag tag, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Tag tag, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}