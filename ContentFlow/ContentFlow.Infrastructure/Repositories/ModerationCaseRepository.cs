using ContentFlow.Application.Common;
using ContentFlow.Application.Interfaces.ModerationCase;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class ModerationCaseRepository : IModerationCaseRepository
{
    private readonly ApplicationDbContext _context;

    public ModerationCaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ModerationCase?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.ModerationCases
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<ModerationCase?> GetOpenByPostIdAsync(int postId, CancellationToken ct)
    {
        return await _context.ModerationCases
            .FirstOrDefaultAsync(
                c => c.PostId == postId &&
                     (c.Status == ModerationCaseStatus.Open || c.Status == ModerationCaseStatus.InReview),
                ct);
    }

    public async Task<ModerationCase?> GetOpenByCommentIdAsync(int commentId, CancellationToken ct)
    {
        return await _context.ModerationCases
            .FirstOrDefaultAsync(
                c => c.CommentId == commentId &&
                     (c.Status == ModerationCaseStatus.Open || c.Status == ModerationCaseStatus.InReview),
                ct);
    }

    public async Task<PaginatedResult<ModerationCase>> GetOpenCasesAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.ModerationCases
            .AsNoTracking()
            .Where(c => c.Status == ModerationCaseStatus.Open || c.Status == ModerationCaseStatus.InReview);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.Priority)
            .ThenByDescending(c => c.LastReportedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedResult<ModerationCase>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(ModerationCase moderationCase, CancellationToken ct)
    {
        await _context.ModerationCases.AddAsync(moderationCase, ct);
    }

    public Task UpdateAsync(ModerationCase moderationCase, CancellationToken ct)
    {
        var entry = _context.Entry(moderationCase);
        if (entry.State == EntityState.Added)
            return Task.CompletedTask;

        if (entry.State == EntityState.Detached)
            _context.ModerationCases.Update(moderationCase);

        return Task.CompletedTask;
    }
}
