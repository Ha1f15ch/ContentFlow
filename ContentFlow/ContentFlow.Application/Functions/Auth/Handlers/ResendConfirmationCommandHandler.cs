using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class ResendConfirmationCommandHandler : IRequestHandler<ResendConfirmationCommand, AuthResult>
{
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    private readonly IUserService _userService;
    private readonly IEmailSender _emailSender;

    public ResendConfirmationCommandHandler(
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository, 
        IUserService userService,
        IEmailSender emailSender)
    {
        _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
        _userService = userService;
        _emailSender = emailSender;
    }

    public async Task<AuthResult> Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return new AuthResult(false, Errors: new() {"User not found"});
        }
        
        var code = await _userTwoFactorCodeRepository.GetValidByUserIdAndPurposeAsync(user.Id, "EmailVerification", cancellationToken);
        if(code != null && DateTime.UtcNow < code.NextResendAt)
        {
            return new AuthResult(false, Errors: new() {$"Resend available after {code.NextResendAt}"});
        }

        if (code != null)
        {
            await _userTwoFactorCodeRepository.MarkAsUsedAsync(code.Id, cancellationToken);
        }
        
        var newCode = GenerateSixDigitCode();
        var (codeHash, codeSalt) = PasswordHasher.Hash(newCode);

        try
        {
            await _userTwoFactorCodeRepository.AddAsync(
                userId: user.Id,
                codeHash: codeHash,
                codeSalt: codeSalt,
                purpose: "EmailVerification",
                ct: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save new verification code: {ex.Message}");
            return new AuthResult(false, Errors: new() { "Failed to generate verification code" });
        }
        
        try
        {
            await _emailSender.SendVerificationEmailAsync(user.Email, newCode, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
        }
        
        return new AuthResult(true);
    }
    
    private static string GenerateSixDigitCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}