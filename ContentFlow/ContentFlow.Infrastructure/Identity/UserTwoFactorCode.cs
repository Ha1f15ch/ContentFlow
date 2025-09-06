using System.ComponentModel.DataAnnotations.Schema;

namespace ContentFlow.Infrastructure.Identity;

public class UserTwoFactorCode
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CodeHash { get; set; }
    public string CodeSalt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; }
    public bool IsUsed { get; set; } = false;
    public string Purpose { get; set; }
    public int ResendCount { get; set; }
    public DateTime NextResendAt { get; set; }
}