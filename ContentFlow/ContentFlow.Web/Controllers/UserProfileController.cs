using ContentFlow.Application.DTOs.UserProfileDTOs;
using ContentFlow.Application.Functions.Subscriptions.Queries;
using ContentFlow.Application.Functions.UserProfile.Commands;
using ContentFlow.Application.Functions.UserProfile.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContentFlow.Application.Security;
using ContentFlow.Domain.Exceptions;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/userprofile")]
[Authorize(Policy = "CanEditContent")]
public class UserProfileController : ControllerBase
{
    private readonly ILogger<UserProfileController> _logger;
    private readonly IMediator _mediator;

    public UserProfileController(
        ILogger<UserProfileController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    /// <summary>
    /// Получить профиль пользователя
    /// </summary>
    [HttpGet("{userProfileId:int}")]
    public async Task<IActionResult> GetUserProfileAsync(int userProfileId)
    {
        var requesterUserId = User.GetAuthenticatedUserId();
        _logger.LogInformation("Fetching user profile - {UserprofileId} for user ID: {UserId}", userProfileId, requesterUserId);

        try
        {
            var query = new GetUserProfileQuery(requesterUserId, userProfileId);
            var profile = await _mediator.Send(query);

            _logger.LogInformation("Profile user - {UserProfile} founded, for user Id: {RequesterUserId}",
                profile.UserId, requesterUserId);

            return Ok(profile);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Profile not found for user ID: {UserId}", userProfileId);
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Access denied to profile for user ID: {UserId}. Error - {ErrorMessage}", userProfileId, ex.Message);
            return Unauthorized(new { message = $"Access denied to profile for user ID: {userProfileId}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch profile for user ID: {UserId}", userProfileId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    /// <summary>
    /// Обновить свой профиль
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateMyProfileAsync([FromBody] UpdateUserProfileRequest request)
    {
        var userId = User.GetAuthenticatedUserId();
        _logger.LogInformation("Updating profile for user ID: {UserId}", userId);
        
        var command = new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            request.BirthDate,
            request.City,
            request.Bio,
            request.Gender);
        
        if (command.UserId != userId)
        {
            _logger.LogWarning("User {ActualUserId} attempted to update profile of user {TargetUserId}", userId, command.UserId);
            return Forbid();
        }

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Profile not found for update (User ID: {UserId})", userId);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data provided for profile update (User ID: {UserId}): {Message}", userId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user ID: {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    /// <summary>
    /// Удалить (деактивировать) свой профиль
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteMyProfileAsync([FromBody] DeleteUserProfileRequest request)
    {
        var userId = User.GetAuthenticatedUserId();
        _logger.LogInformation("Request to delete (mark as deleted) profile for user ID: {UserId}", userId);

        try
        {
            var command = new DeleteUserProfileCommand(userId);
            
            if (command.UserId != userId)
            {
                _logger.LogWarning("User {ActualUserId} attempted to update profile of user {TargetUserId}", userId, command.UserId);
                return Forbid();
            }
            
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Profile deleted for user ID: {UserId}", userId);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to delete profile for user ID: {UserId}. ErrorMessage: {errorMessage}", userId,  result.ErrorMessage);
                return Ok(result);
            }
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Profile not found for deletion (User ID: {UserId})", userId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete profile for user ID: {UserId}", userId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatarAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "File is required." });
        }

        var userId = User.GetAuthenticatedUserId();
        _logger.LogInformation("Uploading avatar for user ID: {UserId}", userId);

        try
        {
            var command = new UpdateUserAvatarCommand(userId, file);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid avatar upload by user {UserId}: {Message}", userId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload avatar for user {UserId}", userId);
            return StatusCode(500, new { message = "Failed to process image." });
        }
    }

    // Мои подписчики
    [HttpGet("my-followers")]
    public async Task<IActionResult> GetMyFollowersAsync()
    {
        var userId = User.GetAuthenticatedUserId();

        try
        {
            var commands = new GetFollowingProfileDtoQuery(userId);
            var result = await _mediator.Send(commands);

            _logger.LogInformation("Following profiles for user ID: {UserId}. Count is {CountFollowers}", userId,
                result.Count);

            return Ok(result);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogWarning(ex, "User profile not found for user ID: {UserId}", userId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to getting userid {UserId} followers", userId);
            return StatusCode(500, new { message = "Failed to process getting followers." });
        }
    }
    
    // Мои подписки
    [HttpGet("my-following")]
    public async Task<IActionResult> GetMyFollowingAsync()
    {
        var userId = User.GetAuthenticatedUserId();
        
        try
        {
            var commands = new GetFollowersProfileDtoQuery(userId);
            var result = await _mediator.Send(commands);

            _logger.LogInformation("Subscription profiles for user ID: {UserId}. Count is {CountFollowers}", userId,
                result.Count);

            return Ok(result);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogWarning(ex, "User profile not found for user ID: {UserId}", userId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to getting userid {UserId} followers", userId);
            return StatusCode(500, new { message = "Failed to process getting followers." });
        }
    }
}