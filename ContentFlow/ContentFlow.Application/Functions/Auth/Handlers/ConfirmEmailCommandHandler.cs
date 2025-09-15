using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
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
        
        var codeDto = await _userTwoFactorCodeRepository.GetValidByPlainCodeAsync(request.Code, "EmailVerification", cancellationToken);

        if (codeDto == null)
        {
            return new AuthResult(false, Errors: new() {"Invalid or expired verification code"});
        }

        if (codeDto.UserId != user.Id)
        {
            return new AuthResult(false, Errors: new() {"Code does not belong to this user"});
        }
        
        await _userService.ConfirmEmailAsync(user.Id, cancellationToken);
        
        await _userTwoFactorCodeRepository.MarkAsUsedAsync(codeDto.Id, cancellationToken);
        
        return new AuthResult(true, null);
    }
}