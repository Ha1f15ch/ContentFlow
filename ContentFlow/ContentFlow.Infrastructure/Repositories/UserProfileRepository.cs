using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Repositories;

public class UserProfileRepository :  IUserProfileRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserProfileRepository> _logger;

    public UserProfileRepository(
        ApplicationDbContext context,
        ILogger<UserProfileRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<UserProfile?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    public async Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken ct = default)
    {
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync(ct);
        return profile;
    }

    public async Task<UserProfile> UpdateAsync(UserProfile profile, CancellationToken ct = default)
    {
        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync(ct);
        return profile;
    }

    public async Task DeleteAsync(int userId, CancellationToken ct = default)
    {
        var profile = await GetByUserIdAsync(userId, ct);
        if (profile != null)
        {
            profile.MarkDeleted();
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.UserProfiles.AnyAsync(p => p.UserId == userId, ct);
    }
}