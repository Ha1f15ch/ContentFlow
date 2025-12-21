using ContentFlow.Application.DTOs.SubscriptionDTOs;
using ContentFlow.Application.Functions.Subscriptions.Commands;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize(Policy = "CanEditContent")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        IMediator mediator,
        ILogger<SubscriptionController> logger)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    /// <summary>
    /// Подписаться на пользователя
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        var userId = User.GetAuthenticatedUserId();

        try
        {
            var command = new SubscribeCommand(userId, request.FollowingUserId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully subscribed to user {FollowingId}", userId, request.FollowingUserId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid subscription request from user {FollowerId} to {FollowingId}: {Message}", 
                userId, request.FollowingUserId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation: user {FollowerId} cannot subscribe to {FollowingId}: {Message}", 
                userId, request.FollowingUserId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while user {FollowerId} subscribing to {FollowingId}", userId, request.FollowingUserId);
            return StatusCode(500, new { message = "Failed to process subscription." });
        }
    }

    /// <summary>
    /// Отписаться от пользователя
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
    {
        var followerId = User.GetAuthenticatedUserId();
        var followingId = request.FollowingId;

        _logger.LogInformation("User {FollowerId} initiated unsubscription from user {FollowingId}", followerId, followingId);

        try
        {
            var command = new UnsubscribeCommand(followerId, followingId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully unsubscribed from user {FollowingId}", followerId, followingId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User {FollowerId} tried to unsubscribe from {FollowingId}, but operation is invalid: {Message}", 
                followerId, followingId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while user {FollowerId} unsubscribing from {FollowingId}", followerId, followingId);
            return StatusCode(500, new { message = "Failed to process unsubscription." });
        }
    }

    /// <summary>
    /// Поставить подписку на паузу
    /// </summary>
    [HttpPut("pause")]
    public async Task<IActionResult> PauseSubscription([FromBody] PauseSubscriptionRequest request)
    {
        var followerId = User.GetAuthenticatedUserId();
        var followingId = request.FollowingUserId;

        _logger.LogInformation("User {FollowerId} initiated pause of subscription to user {FollowingId}", followerId, followingId);

        try
        {
            var command = new PauseSubscriptionCommand(followerId, followingId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully paused subscription to user {FollowingId}", followerId, followingId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot pause subscription: user {FollowerId} to {FollowingId} is not active: {Message}", 
                followerId, followingId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while pausing subscription from {FollowerId} to {FollowingId}", followerId, followingId);
            return StatusCode(500, new { message = "Failed to pause subscription." });
        }
    }

    /// <summary>
    /// Снять подписку с паузы
    /// </summary>
    [HttpPut("resume")]
    public async Task<IActionResult> ResumeSubscription([FromBody] ResumeSubscriptionRequest request)
    {
        var followerId = User.GetAuthenticatedUserId();
        var followingId = request.FollowingUserId;

        _logger.LogInformation("User {FollowerId} initiated resume of subscription to user {FollowingId}", followerId, followingId);

        try
        {
            var command = new ResumeSubscriptionCommand(followerId, followingId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully resumed subscription to user {FollowingId}", followerId, followingId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot resume subscription: user {FollowerId} to {FollowingId} is not paused or inactive: {Message}", 
                followerId, followingId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while resuming subscription from {FollowerId} to {FollowingId}", followerId, followingId);
            return StatusCode(500, new { message = "Failed to resume subscription." });
        }
    }

    /// <summary>
    /// Включить уведомления для подписки
    /// </summary>
    [HttpPut("enable-notifications")]
    public async Task<IActionResult> EnableNotifications([FromBody] EnableNotificationsRequest request)
    {
        var followerId = User.GetAuthenticatedUserId();
        var followingId = request.FollowingUserId;

        _logger.LogInformation("User {FollowerId} initiated enabling notifications for subscription to user {FollowingId}", followerId, followingId);

        try
        {
            var command = new EnableNotificationsCommand(followerId, followingId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully enabled notifications for subscription to user {FollowingId}", followerId, followingId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot enable notifications: subscription from {FollowerId} to {FollowingId} is inactive: {Message}", 
                followerId, followingId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while enabling notifications for subscription from {FollowerId} to {FollowingId}", followerId, followingId);
            return StatusCode(500, new { message = "Failed to enable notifications." });
        }
    }

    /// <summary>
    /// Отключить уведомления для подписки
    /// </summary>
    [HttpPut("disable-notifications")]
    public async Task<IActionResult> DisableNotifications([FromBody] DisableNotificationsRequest request)
    {
        var followerId = User.GetAuthenticatedUserId();
        var followingId = request.FollowingUserId;

        _logger.LogInformation("User {FollowerId} initiated disabling notifications for subscription to user {FollowingId}", followerId, followingId);

        try
        {
            var command = new DisableNotificationsCommand(followerId, followingId);
            await _mediator.Send(command);
            
            _logger.LogInformation("User {FollowerId} successfully disabled notifications for subscription to user {FollowingId}", followerId, followingId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while disabling notifications for subscription from {FollowerId} to {FollowingId}", followerId, followingId);
            return StatusCode(500, new { message = "Failed to disable notifications." });
        }
    }
}