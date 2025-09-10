namespace ContentFlow.Application.Interfaces.Common;

public interface IEmailSender
{
    Task<bool> SendVerificationEmailAsync(string email, string code, CancellationToken ct = default);
    Task<bool> SendPasswordResetEmailAsync(string email, string code, CancellationToken ct = default);
}