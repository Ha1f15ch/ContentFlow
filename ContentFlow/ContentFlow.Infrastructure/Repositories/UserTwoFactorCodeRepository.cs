using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Infrastructure.DatabaseEngine;
using ContentFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using ContentFlow.Application.Security;

namespace ContentFlow.Infrastructure.Repositories;

public class UserTwoFactorCodeRepository : IUserTwoFactorCodeRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserTwoFactorCodeRepository(ApplicationDbContext context)
    {
            _context  = context;
    }

    public async Task AddAsync(int userId, string codeHash, string codeSalt, string purpose, CancellationToken ct)
    {
        var code = new UserTwoFactorCode
        {
            UserId = userId,
            CodeHash = codeHash,
            CodeSalt = codeSalt,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 0,
            MaxAttempts = 5,
            ResendCount = 0,
            NextResendAt = DateTime.UtcNow.AddSeconds(30)
        };

        await _context.UserTwoFactorCodes.AddAsync(code, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<TwoFactorCodeDto?> GetValidByUserIdAndPurposeAsync(int userId, string purpose, CancellationToken ct)
    {
        var code = await _context.UserTwoFactorCodes
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.Purpose == purpose && 
                !c.IsUsed &&
                c.ExpiresAt > DateTime.UtcNow &&
                c.AttemptCount < c.MaxAttempts, ct);

        return code == null ? null : MapToDto(code);
    }

    public async Task<TwoFactorCodeDto?> GetValidByPlainCodeAsync(string plainCode, string purpose, CancellationToken ct)
    {
        var candidates = await _context.UserTwoFactorCodes
            .Where(c =>
                c.Purpose == purpose &&
                !c.IsUsed &&
                c.ExpiresAt > DateTime.UtcNow &&
                c.AttemptCount < c.MaxAttempts)
            .ToListAsync(ct);

        foreach (var candidate in candidates)
        {
            if (PasswordHasher.Verify(plainCode, candidate.CodeSalt, candidate.CodeHash))
                return MapToDto(candidate);
        }

        return null;
    }

    public async Task<bool> IncrementAttemptAsync(int codeId, CancellationToken ct)
    {
        var code = await _context.UserTwoFactorCodes.FindAsync([codeId], ct);
        if (code == null) return false;

        code.AttemptCount++;
        _context.UserTwoFactorCodes.Update(code);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task MarkAsUsedAsync(int codeId, CancellationToken ct)
    {
        var code = await _context.UserTwoFactorCodes.FindAsync([codeId], ct);
        if (code == null) return;

        code.IsUsed = true;
        _context.UserTwoFactorCodes.Update(code);
        await _context.SaveChangesAsync(ct);
    }

    private TwoFactorCodeDto MapToDto(UserTwoFactorCode code)
    {
        return new TwoFactorCodeDto(
            Id: code.Id,
            UserId: code.UserId,
            ExpiresAt: code.ExpiresAt,
            CreatedAt: code.CreatedAt,
            AttemptCount: code.AttemptCount,
            MaxAttempts: code.MaxAttempts,
            IsUsed: code.IsUsed,
            Purpose: code.Purpose,
            NextResendAt: code.NextResendAt);
    }
}