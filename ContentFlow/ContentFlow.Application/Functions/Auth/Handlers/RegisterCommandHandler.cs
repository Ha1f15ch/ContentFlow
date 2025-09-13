using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using FluentValidation;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserService  _userService;
    private readonly IEmailSender  _emailSender;
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    
    public RegisterCommandHandler(
        IUserService userService, 
        IEmailSender emailSender, 
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository)
    {
            _emailSender  = emailSender;
            _userService = userService;
            _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userService.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            return new AuthResult(Success: false, Errors: new() {$"User with email {request.Email} already exists"});
        }
        else
        {
            UserDto userDto;
            try
            {
                userDto = await _userService.CreateAsync(request.Email, request.Password, request.FirstName,
                    request.LastName);
            }
            catch (ValidationException validationException)
            {
                Console.WriteLine($"Operation failed: {validationException.Message}");
                return new AuthResult(Success: false, Errors: new() {$"{validationException.Errors}"});
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Unhandled exception: {exception.Message}");
                return new AuthResult(Success: false, Errors: new() {$"{exception.Message}"});
            }
            
            // Creating 2FA code
            var code = GenerateSixValueCode();

            var (codeHash, codeSalt) = PasswordHasher.Hash(code);

            try
            {
                await _userTwoFactorCodeRepository.AddAsync(
                    userId: userDto.Id,
                    codeHash: codeHash,
                    codeSalt: codeSalt,
                    purpose: "EmailVerification",
                    ct: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save verification code: {ex.Message}");
                return new AuthResult(false, Errors: new() { "Failed to initiate email verification" });
            }
            
            // try sand code to email
            try
            {
                var emailSand = await _emailSender.SendVerificationEmailAsync(userDto.Email, code, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}"); //user can resend code again later
            }
            
            return new AuthResult(true);
        }
    }
    
    private static string GenerateSixValueCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}