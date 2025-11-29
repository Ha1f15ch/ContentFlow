using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.RefreshToken;

public interface ITokenService
{
    public string GenerateToken(int userId, string email, IEnumerable<string> roles);
}