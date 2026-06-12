using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Domain.Entities;
using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
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

    public async Task<UserProfile?> GetByIdAsync(int userProfileId, CancellationToken ct = default)
    {
        return await _context.UserProfiles.FindAsync(userProfileId);
    }

    public async Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken ct = default)
    {
        await _context.UserProfiles.AddAsync(profile, ct);
        return profile;
    }

    public Task<UserProfile> UpdateAsync(UserProfile profile, CancellationToken ct = default)
    {
        _context.UserProfiles.Update(profile);
        return Task.FromResult(profile);
    }

    public async Task DeleteAsync(int userId, CancellationToken ct = default)
    {
        var profile = await GetByUserIdAsync(userId, ct);
        if (profile != null)
        {
            profile.MarkDeleted();
        }
    }

    public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.UserProfiles.AnyAsync(p => p.UserId == userId, ct);
    }
}