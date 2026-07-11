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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterCommandHandler> _logger;
    
    public RegisterCommandHandler(
        IUserService userService, 
        IEmailSender emailSender, 
        IUserTwoFactorCodeRepository userTwoFactorCodeRepository,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<RegisterCommandHandler> logger)
    {
            _emailSender  = emailSender;
            _userService = userService;
            _userTwoFactorCodeRepository = userTwoFactorCodeRepository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User registration started for email: {Email}", request.Email);
        
        var existingUser = await _userService.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser != null)
        {
            if (await _userService.IsSelfDeletedAccountAsync(existingUser.Id, cancellationToken))
            {
                _logger.LogInformation("Registration blocked: deleted account exists for email: {Email}", request.Email);
                return new AuthResult(
                    Success: false,
                    Errors: "Account was deleted.",
                    AccountDeleted: true,
                    Message: "An account with this email was deleted. You can restore it with the same password.");
            }

            _logger.LogWarning("Registration attempt with already registered email: {Email}", request.Email);
            return new AuthResult(Success: false, Errors: $"User with email {request.Email} already exists");
        }

        var existingByUserName = await _userService.GetUserByUserNameAsync(request.UserName, cancellationToken);
        if (existingByUserName != null)
        {
            if (await _userService.IsSelfDeletedAccountAsync(existingByUserName.Id, cancellationToken))
            {
                _logger.LogInformation("Registration blocked: deleted account exists for username: {UserName}", request.UserName);
                return new AuthResult(
                    Success: false,
                    Errors: "Account was deleted.",
                    AccountDeleted: true,
                    Message: "An account with this username was deleted. Restore it or choose another username.");
            }

            return new AuthResult(Success: false, Errors: "Username is already taken.");
        }

        UserDto userDto;
        try
        {
            userDto = await _userService.CreateAsync(request.Email, request.Password, request.UserName);
            _logger.LogInformation("User created successfully with ID: {UserId}", userDto.Id);
        }
        catch (ValidationException validationException)
        {
            _logger.LogError(validationException, "Validation failed during registration for email: {Email}", request.Email);
            return new AuthResult(Success: false, Errors: $"{validationException.Errors}");
        }
        catch (Exception exception)
        {
            _logger.LogCritical(exception, "Unexpected error creating user with email: {Email}", request.Email);
            return new AuthResult(Success: false, Errors: $"{exception.Message}");
        }
        
        _logger.LogInformation("Creating user profile for user ID: {UserId}", userDto.Id);

        var profile = new Domain.Entities.UserProfile(userId: userDto.Id);
        await _userProfileRepository.CreateAsync(profile, cancellationToken);
        
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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("UserProfile created successfully with ID: {UserProfileId} for user ID: {UserId}", 
                profile.Id, userDto.Id);
            _logger.LogInformation("Verification code generated and saved for user: {UserId}", userDto.Id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to save verification code for user: {UserId}", userDto.Id);
            return new AuthResult(false, Errors: "Failed to initiate email verification");
        }
        
        await _userService.AddToRoleAsync(userDto.Email, RoleConstants.Guest.ToString(), cancellationToken);
        _logger.LogInformation("User {UserId} assigned to role: {Role}", userDto.Id, RoleConstants.Guest);
        
        try
        {
            var emailSend = await _emailSender.SendVerificationEmailAsync(userDto.Email, code, cancellationToken);

            if (emailSend)
            {
                _logger.LogInformation("Verification email sent to: {Email}", userDto.Email);
                _logger.LogInformation("User registration completed successfully: {Email}", request.Email);
                return new AuthResult(
                    Success: true,
                    RequiresEmailConfirmation: true,
                    EmailSent: true,
                    Message: "Verification email sent.");
            }

            _logger.LogWarning(
                "User {Email} registered, but verification email was not sent",
                request.Email);

            return new AuthResult(
                Success: true,
                RequiresEmailConfirmation: true,
                EmailSent: false,
                Message: "Account created, but the verification email could not be sent. Please resend the code.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "User {Email} registered, but sending verification email failed. User can resend later.",
                userDto.Email);

            return new AuthResult(
                Success: true,
                RequiresEmailConfirmation: true,
                EmailSent: false,
                Message: "Account created, but the verification email could not be sent. Please resend the code.");
        }
    }
}