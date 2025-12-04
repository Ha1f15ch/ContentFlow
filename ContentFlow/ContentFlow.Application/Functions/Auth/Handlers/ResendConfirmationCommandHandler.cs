using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class ResendConfirmationCommandHandler : IRequestHandler<ResendConfirmationCommand, AuthResult>
{
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ResendConfirmationCommandHandler> _logger;

    public ResendConfirmationCommandHandler(
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository, 
        IUserService userService,
        IEmailSender emailSender,
        ILogger<ResendConfirmationCommandHandler> logger)
    {
        _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
        _userService = userService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resend confirmation email requested for: {Email}", request.Email);
        
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Resend failed: user not found - {Email}", request.Email);
            return new AuthResult(false, Errors: "User not found");
        }
        
        var code = await _userTwoFactorCodeRepository.GetValidByUserIdAndPurposeAsync(user.Id, "EmailVerification", cancellationToken);
        if(code != null && DateTime.UtcNow < code.NextResendAt)
        {
            _logger.LogWarning("Resend blocked by cooldown for user: {Email}. Available after: {NextResendAt}", request.Email, code.NextResendAt);
            return new AuthResult(false, Errors: $"Resend available after {code.NextResendAt}");
        }

        if (code != null)
        {
            await _userTwoFactorCodeRepository.MarkAsUsedAsync(code.Id, cancellationToken);
            _logger.LogInformation("Previous verification code marked as used for user: {UserId}", user.Id);
        }
        
        var newCode = TokenGenerator.GenerateSixValueCode();
        var (codeHash, codeSalt) = PasswordHasher.Hash(newCode);

        try
        {
            await _userTwoFactorCodeRepository.AddAsync(
                userId: user.Id,
                codeHash: codeHash,
                codeSalt: codeSalt,
                purpose: "EmailVerification",
                ct: cancellationToken);
            _logger.LogInformation("New verification code generated and saved for user: {UserId}", user.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save new verification code: {ex.Message}");
            return new AuthResult(false, Errors: "Failed to generate verification code");
        }
        
        try
        {
            await _emailSender.SendVerificationEmailAsync(user.Email, newCode, cancellationToken);
            _logger.LogInformation("Confirmation email resent successfully to: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to: {Email}", user.Email);
            return new AuthResult(false, Errors: "Email sending failed");
        }
        
        _logger.LogInformation("Resend confirmation flow completed for: {Email}", request.Email);
        return new AuthResult(true);
    }
}