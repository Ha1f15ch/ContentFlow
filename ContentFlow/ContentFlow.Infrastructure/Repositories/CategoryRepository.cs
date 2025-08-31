using ContentFlow.Application.Common;
using ContentFlow.Application.Interfaces.Category;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Category> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Category with id {id} not found");
    }

    public async Task DeleteAsync(Category category, CancellationToken ct)
    {
        _context.Categories.Remove(category);
        await  _context.SaveChangesAsync(ct);
    }

    public async Task<PaginatedResult<Category>> GetPaginatedAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _context.Categories.CountAsync(ct);
        
        var categories = await _context.Categories
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PaginatedResult<Category>(categories, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(Category category, CancellationToken ct)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddAsync(Category category, CancellationToken ct)
    {
        await _context.Categories.AddAsync(category, ct);
        await _context.SaveChangesAsync(ct);
    }
}