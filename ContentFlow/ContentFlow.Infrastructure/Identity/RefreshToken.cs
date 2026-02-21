using System.ComponentModel.DataAnnotations.Schema;

namespace ContentFlow.Infrastructure.Identity;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenLookupHash { get; set; } = default!;
    public string TokenHash { get; set; }
    public string TokenSalt { get; set; }
    public DateTime ExpiresAt  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? DeviceId { get; set; }
    public bool IsRevoked { get; set; } = false;
}