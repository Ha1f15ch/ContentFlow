using System.Security.Cryptography;

namespace ContentFlow.Application.Security;

public class TokenGenerator
{
    public static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
    
    public static string GenerateSixValueCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}