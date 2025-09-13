using System.Globalization;
using System.Net;
using System.Net.Mail;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MimeKit.Text;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ContentFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task SendAsync(
        string to, 
        string subject, 
        string body, 
        bool isHtml = false, 
        CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.From, _emailSettings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        if (isHtml)
            bodyBuilder.HtmlBody = body;
        else
            bodyBuilder.TextBody = body;

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.LocalDomain = "localhost";

            Console.WriteLine($"Пытаемся установить соединение с почтой. HOST: {_emailSettings.Host}, PORT: {_emailSettings.Port}");
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.Auto, ct);

            Console.WriteLine($"Аутентификация в сервисе почты");
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, ct);

            Console.WriteLine($"Пытаемся отправить письмо на email: {to}");
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            Console.WriteLine($"Письмо успешно отправлено на {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке письма на {to}: {ex.Message}");
            throw new Exception($"Не удалось отправить письмо на {to}", ex);
        }
    }
}