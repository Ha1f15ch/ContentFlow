using ContentFlow.Application.DTOs;
using ContentFlow.Application.DTOs.PrivateDTOModels;

namespace ContentFlow.Application.Interfaces.Users;

public interface IUserTwoFactorCodeRepository
{
    public Task AddAsync(int userId, string codeHash, string codeSalt, string purpose, CancellationToken ct);

    public Task<TwoFactorCodeDto?> GetValidByUserIdAndPurposeAsync(int userId, string purpose, CancellationToken ct);

    public Task<TwoFactorCodeDto?> GetValidByPlainCodeAsync(string plainCode, int userId, string purpose, CancellationToken ct);
    
    Task<VerificationCodeVerificationDto?> GetVerificationCodeForValidationAsync(int userId, string purpose, CancellationToken ct);
    
    public Task<bool> IncrementAttemptAsync(int codeId, CancellationToken ct);

    public Task MarkAsUsedAsync(int codeId, CancellationToken ct);
}