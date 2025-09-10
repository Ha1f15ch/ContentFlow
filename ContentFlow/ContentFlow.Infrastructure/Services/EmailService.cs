using System.Net;
using System.Net.Mail;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ContentFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken ct = default)
    {
        try
        {
            using var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };
            
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);
            
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromMinutes(1));
            
            await Task.Run(() => smtpClient.Send(message), cts.Token);
            
            Console.WriteLine($"Письмо успешно отправлено на {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке письма на {to}: {ex.Message}");
            throw new Exception($"Не удалось отправить письмо на {to}", ex);
        }
    }
}