using System.ComponentModel.DataAnnotations.Schema;

namespace ContentFlow.Infrastructure.Identity;

public class TrustedDevice
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DeviceId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
}