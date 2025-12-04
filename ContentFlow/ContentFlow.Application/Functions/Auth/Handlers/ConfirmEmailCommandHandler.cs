using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class ConfirmEmailCommandHandler :  IRequestHandler<ConfirmEmailCommand, AuthResult>
{
    private readonly IEmailSender _emailSender;
    private readonly IUserService _userService;
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;
    
    public ConfirmEmailCommandHandler(
        IEmailSender emailSender,
        IUserService userService,
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository,
        ILogger<ConfirmEmailCommandHandler> logger)
    {
        _emailSender = emailSender;
        _userService = userService;
        _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email confirmation started for user: {Email}", request.Email);
        
        var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Email confirmation failed: user not found - {Email}", request.Email);
            return new AuthResult(false, Errors: "User not found");
        }
        
        var activeCodeDto = await _userTwoFactorCodeRepository.GetVerificationCodeForValidationAsync(user.Id, "EmailVerification", cancellationToken);

        if (activeCodeDto == null)
        {
            _logger.LogWarning("No active verification code found for user: {UserId}", user.Id);
            return new AuthResult(false, Errors: "No active verification code");
        }

        bool isCodeValid = PasswordHasher.Verify(
            input: request.Code,
            salt: activeCodeDto.CodeSalt,
            hash: activeCodeDto.CodeHash);

        if (!isCodeValid)
        {
            await _userTwoFactorCodeRepository.IncrementAttemptAsync(activeCodeDto.Id, cancellationToken);
            _logger.LogWarning("Invalid verification code provided by user: {Email}", request.Email);
            return new AuthResult(false, Errors: "Invalid verification code");
        }
        
        var setEmailConfirmed = await _userService.ConfirmEmailAsync(user.Id, cancellationToken);

        if (!setEmailConfirmed)
        {
            _logger.LogError("Failed to confirm email in user service for user: {UserId}", user.Id);
            return new AuthResult(false, Errors: "Email not confirmed");
        }
        
        await _userTwoFactorCodeRepository.MarkAsUsedAsync(activeCodeDto.Id, cancellationToken);
        _logger.LogInformation("Verification code marked as used for user: {UserId}", user.Id);
        
        await _userService.RemoveFromRoleAsync(user.Email, RoleConstants.Guest.ToString(), cancellationToken);
        await _userService.AddToRoleAsync(user.Email, RoleConstants.User.ToString(), cancellationToken);
        
        _logger.LogInformation("User {UserId} role updated: Guest → User after email confirmation", user.Id);
        
        _logger.LogInformation("Email confirmed successfully for user: {Email}", request.Email);
        return new AuthResult(true, null);
    }
}