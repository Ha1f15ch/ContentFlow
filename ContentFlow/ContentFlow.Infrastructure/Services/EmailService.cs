using System.Diagnostics;
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
using Microsoft.Extensions.Logging;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ContentFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
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

        _logger.LogDebug("Attempting to connect to SMTP server: {Host}:{Port}", _emailSettings.Host, _emailSettings.Port);
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.LocalDomain = "localhost";

            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.Auto, ct);
            _logger.LogDebug("SMTP connection established");

            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, ct);
            _logger.LogDebug("SMTP authentication successful");
            
            await client.SendAsync(message, ct);
            _logger.LogDebug("Email sent via SMTP");
            
            await client.DisconnectAsync(true, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "Email sent successfully to {To} in {ElapsedMs}ms", 
                to, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, 
                "Failed to send email to {To} after {ElapsedMs}ms", 
                to, stopwatch.ElapsedMilliseconds);
            throw new Exception($"Не удалось отправить письмо на {to}", ex);
        }
    }
}