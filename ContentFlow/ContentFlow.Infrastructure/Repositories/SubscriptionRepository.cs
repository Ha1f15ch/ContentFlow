using System.Reflection.Metadata.Ecma335;
using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubscriptionRepository> _logger;
    
    public SubscriptionRepository(ApplicationDbContext context, ILogger<SubscriptionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Subscription?> GetByFollowerAndFollowingAsync(int followerId, int followingId, CancellationToken ct = default)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s =>
            s.UserProfileFollowerId == followerId && s.UserProfileFollowingId == followingId, ct);
    }

    public async Task AddAsync(int followerId, int followingId, CancellationToken ct = default)
    {
        if (followerId == 0 || followingId == 0)
        {
            _logger.LogError("FollowerId == 0 || FollowingId == 0");
            throw new ArgumentNullException(nameof(followerId), $"{nameof(followerId)} cannot be null or empty.");
        }

        var newSubscriptionRecord = new Subscription(followerId, followingId);
        
        _logger.LogInformation($"Adding subscription record {newSubscriptionRecord}");
        
        await _context.Subscriptions.AddAsync(newSubscriptionRecord, ct);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation($"Subscription record {newSubscriptionRecord} added");
    }

    public async Task<Subscription?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if(id > 0)
            return await _context.Subscriptions.FindAsync(id, ct);
        
        _logger.LogError($"Subscription {id} not found");
        return null;
    }

    /// <summary>
    /// Список тех, на кого подписан пользователь
    /// </summary>
    /// <param name="follower"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<List<SubscriptionWithFollowingProfileDto>> GetListSubscriptionFollowersByFollowerAsync(int follower, CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .Where(s => s.UserProfileFollowerId == follower && s.DeactivatedAt == null)
            .Join(
                _context.UserProfiles, 
                subscription => subscription.UserProfileFollowingId, 
                userProfile => userProfile.Id, 
                (subscription, userProfile) => new SubscriptionWithFollowingProfileDto
                {
                    SubscriptionId = subscription.Id,
                    FollowingProfileId =  userProfile.Id,
                    FollowingFullName = $"{userProfile.LastName} {userProfile.FirstName} {userProfile.MiddleName}".Trim(),
                    FollowingAvatarUrl =  userProfile.AvatarUrl,
                    IsPaused = subscription.IsPaused,
                    IsActive = subscription.IsActive,
                    SubscribedAt =  subscription.CreatedAt
                })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Список тех, кто подписан на пользователя
    /// </summary>
    /// <param name="followingId">На кого подписаны</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<SubscriptionWithFollowerProfileDto>> GetListSubscriptionFollowingAsync(int followingId, CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .Where(s => s.UserProfileFollowingId == followingId && s.DeactivatedAt == null)
            .Join(
                _context.UserProfiles,
                subscription => subscription.UserProfileFollowerId,
                userProfile => userProfile.Id,
                (subscription, userProfile) => new SubscriptionWithFollowerProfileDto
                {
                    SubscriptionId = subscription.Id,
                    FollowerProfileId =  userProfile.Id,
                    FollowerFullName =  $"{userProfile.LastName} {userProfile.FirstName} {userProfile.MiddleName}".Trim(),
                    FollowerAvatarUrl =  userProfile.AvatarUrl,
                    IsPaused = subscription.IsPaused,
                    IsActive = subscription.IsActive,
                    SubscribedAt =  subscription.CreatedAt
                })
            .ToListAsync(ct);
    }
    
    public async Task<List<int>> GetUserIdsWithActiveNotification(int followingId,
        CancellationToken ct = default)
    {
        return await _context.Subscriptions
            .Where(el => el.UserProfileFollowingId == followingId && 
                                   el.DeactivatedAt == null && 
                                   el.NotificationsEnabled)
            .Join(
                _context.UserProfiles,
                subscription => subscription.UserProfileFollowerId,
                userProfile => userProfile.Id,
                (subscription, profile) => profile.UserId)
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}