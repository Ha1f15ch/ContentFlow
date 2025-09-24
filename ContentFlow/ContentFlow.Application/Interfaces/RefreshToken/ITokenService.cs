using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.RefreshToken;

public interface ITokenService
{
    public string GenerateToken(int userId, string email, string? firstName, string? lastName, IEnumerable<string> roles);
}