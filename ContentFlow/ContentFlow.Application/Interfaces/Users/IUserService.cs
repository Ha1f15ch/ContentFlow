using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.Users;

public interface IUserService
{
    Task<UserDto> GetByEmailAsync(string email, CancellationToken ct);
    Task<UserDto> GetByIdAsync(string userId, CancellationToken ct);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<List<UserDto>> GetAllAsync(CancellationToken ct);
}