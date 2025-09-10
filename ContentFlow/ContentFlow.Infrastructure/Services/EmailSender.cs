using ContentFlow.Application.Interfaces.Common;

namespace ContentFlow.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private IEmailService _emailService;

    public EmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task<bool> SendVerificationEmailAsync(string email, string code, CancellationToken ct = default)
    {
        try
        {
            await _emailService.SendAsync(
                to: email,
                subject: "Код для подтверждения аккаунта",
                body: $"<h3>Ваш код: <strong>{code}</strong></h3>",
                isHtml: true,
                ct: ct);

            Console.WriteLine("Код верификации отправлен на {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex}, Ошибка при отправке кода верификации на {email}");
            return false;
        }
    }

    public Task<bool> SendPasswordResetEmailAsync(string email, string code, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}