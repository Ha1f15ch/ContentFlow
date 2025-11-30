namespace ContentFlow.Application.Interfaces.UserProfile;

public interface IUserProfileRepository
{
    Task<Domain.Entities.UserProfile?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Domain.Entities.UserProfile> CreateAsync(Domain.Entities.UserProfile profile, CancellationToken ct = default);
    Task<Domain.Entities.UserProfile> UpdateAsync(Domain.Entities.UserProfile profile, CancellationToken ct = default);
    Task DeleteAsync(int userId, CancellationToken ct = default);
    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken ct = default);
}