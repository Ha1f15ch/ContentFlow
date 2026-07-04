using ContentFlow.Application.Interfaces.Report;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<Report?> GetByReporterAndPostAsync(int reporterId, int postId, CancellationToken ct)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.ReporterId == reporterId && r.PostId == postId, ct);
    }

    public async Task<Report?> GetByReporterAndCommentAsync(int reporterId, int commentId, CancellationToken ct)
    {
        return await _context.Reports
            .FirstOrDefaultAsync(r => r.ReporterId == reporterId && r.CommentId == commentId, ct);
    }

    public async Task<IReadOnlyList<Report>> GetByPostIdAsync(int postId, CancellationToken ct)
    {
        return await _context.Reports
            .AsNoTracking()
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Report>> GetByCommentIdAsync(int commentId, CancellationToken ct)
    {
        return await _context.Reports
            .AsNoTracking()
            .Where(r => r.CommentId == commentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Report report, CancellationToken ct)
    {
        await _context.Reports.AddAsync(report, ct);
    }
}
