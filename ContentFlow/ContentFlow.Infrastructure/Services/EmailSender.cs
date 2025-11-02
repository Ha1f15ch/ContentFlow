using ContentFlow.Application.Interfaces.Common;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private IEmailService _emailService;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IEmailService emailService,
        ILogger<EmailSender> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }
    public async Task<bool> SendVerificationEmailAsync(string email, string code, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending verification email to: {Email}", email);
        
        try
        {
            await _emailService.SendAsync(
                to: email,
                subject: "Код для подтверждения аккаунта",
                body: $"<h3>Ваш код: <strong>{code}</strong></h3>",
                isHtml: true,
                ct: ct);

            _logger.LogInformation("Verification email sent successfully to: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to: {Email}", email);
            return false;
        }
    }

    public Task<bool> SendPasswordResetEmailAsync(string email, string code, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending password reset email to: {Email}", email);
        
        // ToDo Нужно реализовать
        throw new NotImplementedException();
    }
}