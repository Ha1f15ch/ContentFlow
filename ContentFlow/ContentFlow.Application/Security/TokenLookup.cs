using System.Security.Cryptography;
using System.Text;

namespace ContentFlow.Application.Security;

public static class TokenLookup
{
    public static string Sha256Base64(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}