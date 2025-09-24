using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.Users;

public interface IUserService
{
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct);
    Task<List<UserDto>> GetByIdsAsync(List<int> userIds, CancellationToken ct);
    Task<UserDto> GetByIdAsync(int userId, CancellationToken ct);
    Task<UserDto> CreateAsync(string email, string password, string? firstName = null, string? lastName = null);
    Task AddToRoleAsync(string email, string role, CancellationToken ct);
    Task RemoveFromRoleAsync(string email, string role, CancellationToken ct);
    Task<bool> IsInRoleAsync(int userId, string role);
    Task<List<UserDto>> GetAllAsync(CancellationToken ct);
    Task<bool> CheckPasswordAsync(string email, string password, CancellationToken ct);
    Task<bool> ConfirmEmailAsync(int userId, CancellationToken ct);
    Task<List<string>> GetRolesAsync(string email, CancellationToken ct);
}