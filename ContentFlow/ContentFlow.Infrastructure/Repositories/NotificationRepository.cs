using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<NotificationRepository> _logger;
    
    public NotificationRepository(ApplicationDbContext dbContext, ILogger<NotificationRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task AddAsync(Notification notification, CancellationToken ct)
    {
        await _dbContext.Notifications.AddAsync(notification, ct);
    }

    public async Task<IReadOnlyList<Notification>> GetByUserAsync(int userId, int take, CancellationToken ct)
    {
        return await _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<Notification?> GetByIdForUserAsync(int notificationId, int userId, CancellationToken ct)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(int userId, CancellationToken ct)
    {
        return await _dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(ct);
    }
    
    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct)
    {
        await _dbContext.Notifications.AddRangeAsync(notifications, ct);
    }
}