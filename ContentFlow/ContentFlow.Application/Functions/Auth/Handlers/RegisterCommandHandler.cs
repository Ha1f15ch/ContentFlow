using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Auth.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Security;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserService  _userService;
    private readonly IEmailSender  _emailSender;
    private readonly IUserTwoFactorCodeRepository _userTwoFactorCodeRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<RegisterCommandHandler> _logger;
    
    public RegisterCommandHandler(
        IUserService userService, 
        IEmailSender emailSender, 
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository,
        IUserProfileRepository userProfileRepository,
        ILogger<RegisterCommandHandler> logger)
    {
            _emailSender  = emailSender;
            _userService = userService;
            _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User registration started for email: {Email}", request.Email);
        
        var existingUser = await _userService.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempt with already registered email: {Email}", request.Email);
            return new AuthResult(Success: false, Errors: new() {$"User with email {request.Email} already exists"});
        }
        else
        {
            UserDto userDto;
            try
            {
                userDto = await _userService.CreateAsync(request.Email, request.Password, request.UserName);
                _logger.LogInformation("User created successfully with ID: {UserId}", userDto.Id);
            }
            catch (ValidationException validationException)
            {
                _logger.LogError(validationException, "Validation failed during registration for email: {Email}", request.Email);
                return new AuthResult(Success: false, Errors: new() {$"{validationException.Errors}"});
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "Unexpected error creating user with email: {Email}", request.Email);
                return new AuthResult(Success: false, Errors: new() {$"{exception.Message}"});
            }
            
            // Создаем профиль пользователя
            _logger.LogInformation("Creating user profile for user ID: {UserId}", userDto.Id);

            var profile = new Domain.Entities.UserProfile(userId: userDto.Id);
            await _userProfileRepository.CreateAsync(profile, cancellationToken);

            _logger.LogInformation("UserProfile created successfully with ID: {UserProfileId} for user ID: {UserId}", 
                profile.Id, userDto.Id);
            
            // Creating 2FA code
            var code = TokenGenerator.GenerateSixValueCode();
            var (codeHash, codeSalt) = PasswordHasher.Hash(code);

            try
            {
                await _userTwoFactorCodeRepository.AddAsync(
                    userId: userDto.Id,
                    codeHash: codeHash,
                    codeSalt: codeSalt,
                    purpose: "EmailVerification",
                    ct: cancellationToken);
                _logger.LogInformation("Verification code generated and saved for user: {UserId}", userDto.Id);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to save verification code for user: {UserId}", userDto.Id);
                return new AuthResult(false, Errors: new() { "Failed to initiate email verification" });
            }
            
            await _userService.AddToRoleAsync(userDto.Email, RoleConstants.Guest.ToString(), cancellationToken);
            _logger.LogInformation("User {UserId} assigned to role: {Role}", userDto.Id, RoleConstants.Guest);
            
            // try to send code to email
            try
            {
                var emailSend = await _emailSender.SendVerificationEmailAsync(userDto.Email, code, cancellationToken);
                _logger.LogInformation("Verification email sent to: {Email}", userDto.Email);
                
                if (emailSend)
                {
                    _logger.LogInformation("User registration completed successfully: {Email}", request.Email);
                    return new AuthResult(true);
                }
                else
                {
                    _logger.LogInformation("User registration failed: {Email}", request.Email);
                    return new AuthResult(false, Errors: new() { "User registration failed: {Email}", request.Email });
                }
            
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send verification email to: {Email}. User can retry later.", userDto.Email); //user can resend code again later
                return new AuthResult(false, Errors: new() { "Internal server error:",  ex.Message });
            }
        }
    }
}