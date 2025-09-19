using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class ConfirmEmailCommandHandler :  IRequestHandler<ConfirmEmailCommand, AuthResult>
{
    private readonly IEmailSender _emailSender;
    private readonly IUserService _userService;
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    
    public ConfirmEmailCommandHandler(
        IEmailSender emailSender,
        IUserService userService,
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository)
    {
        _emailSender = emailSender;
        _userService = userService;
        _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
    }

    public async Task<AuthResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return new AuthResult(false, Errors: new() {"User not found"});
        }
        
        var activeCodeDto = await _userTwoFactorCodeRepository.GetVerificationCodeForValidationAsync(user.Id, "EmailVerification", cancellationToken);

        if (activeCodeDto == null)
        {
            return new AuthResult(false, Errors: new() { "No active verification code" });
        }

        bool isCodeValid = PasswordHasher.Verify(
            input: request.Code,
            salt: activeCodeDto.CodeSalt,
            hash: activeCodeDto.CodeHash);

        if (!isCodeValid)
        {
            await _userTwoFactorCodeRepository.IncrementAttemptAsync(activeCodeDto.Id, cancellationToken);
            return new AuthResult(false, Errors: new() { "Invalid verification code" });
        }
        
        await _userTwoFactorCodeRepository.MarkAsUsedAsync(activeCodeDto.Id, cancellationToken);
        var setEmailConfirmed = await _userService.ConfirmEmailAsync(user.Id, cancellationToken);

        if (!setEmailConfirmed)
        {
            return new AuthResult(false, Errors: new() { "Email not confirmed" });
        }
        
        return new AuthResult(true, null);
    }
}