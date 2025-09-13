using System.Security.Cryptography;

namespace ContentFlow.Application.Security;

public static class PasswordHasher
{
    /// <summary>
    /// Хэширует строку с использованием Rfc2898DeriveBytes (PBKDF2).
    /// </summary>
    /// <param name="input">Входная строка (например, код)</param>
    /// <returns>Хэш и соль в виде строки Base64</returns>
    public static (string hash, string salt) Hash(string input)
    {
        var saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        var rfc2898 = new Rfc2898DeriveBytes(input, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashBytes = rfc2898.GetBytes(256 / 8);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    /// <summary>
    /// Проверяет, соответствует ли входная строка сохранённому хэшу.
    /// </summary>
    /// <param name="input">Входная строка (введённый пользователем код)</param>
    /// <param name="salt">Соль (из БД)</param>
    /// <param name="hash">Хэш (из БД)</param>
    /// <returns>true, если совпадает</returns>
    public static bool Verify(string input, string salt, string hash)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var rfc2898 = new Rfc2898DeriveBytes(input, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hashBytes = rfc2898.GetBytes(256 / 8);

        return Convert.ToBase64String(hashBytes) == hash;
    }
}