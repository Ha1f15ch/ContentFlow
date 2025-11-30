using ContentFlow.Application.Functions.Administration.Commands;
using ContentFlow.Application.Functions.Administration.Queries;
using ContentFlow.Application.Security;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdministrationDictionary")]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IMediator _mediator;
    private int RequesterId() => User.GetAuthenticatedUserId();
    
    public AdminController(
        ILogger<AdminController> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    /// <summary>
    /// Найти пользователя по email или части имени
    /// </summary>
    [HttpGet("users/search")]
    public async Task<IActionResult> GetUserSearch(
        [FromQuery] string query,
        [FromQuery] int limit = 10)
    {
        _logger.LogInformation("Admin {AdminId} initiated user search with query: '{Query}', limit: {Limit}", 
            RequesterId(), query, limit);
        
        try
        {
            var result = await _mediator.Send(new SearchUsersQuery(query, limit));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user search by admin {AdminId} with query: '{Query}'", 
                RequesterId(), query);
            return StatusCode(500, new { message = "Internal server error during user search." });
        }
    }

    /// <summary>
    /// Забанить пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="banReason"></param>
    /// <returns></returns>
    [HttpPost("users/{userId:int}/ban")]
    public async Task<IActionResult> BanUserAsync(int userId, [FromBody] string banReason)
    {
        _logger.LogInformation("Admin {AdminId} attempting to ban user {UserId} with reason: '{Reason}'", 
            RequesterId(), userId, banReason);

        try
        {
            await _mediator.Send(new BanUserCommand(userId, banReason, RequesterId()));
            _logger.LogInformation("User {UserId} successfully banned by admin {AdminId}", userId, RequesterId());
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Attempt to ban non-existent user {UserId} by admin {AdminId}", 
                userId, RequesterId());
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error during ban of user {UserId} by admin {AdminId}: {Message}", 
                userId, RequesterId(), ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while banning user {UserId} by admin {AdminId}", 
                userId, RequesterId());
            return StatusCode(500, new { message = "Failed to ban user." });
        }
    }

    /// <summary>
    /// Разбанить пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("users/{userId:int}/unban")]
    public async Task<IActionResult> UnbanUserAsync(int userId)
    {
        _logger.LogInformation("Admin {AdminId} attempting to unban user {UserId}", RequesterId(), userId);

        try
        {
            var result = await _mediator.Send(new UnbanUserCommand(userId, RequesterId()));
            _logger.LogInformation("User {UserId} successfully unbanned by admin {AdminId}", userId, RequesterId());
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Attempt to unban non-existent user {UserId} by admin {AdminId}", 
                userId, RequesterId());
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error during unban of user {UserId} by admin {AdminId}: {Message}", 
                userId, RequesterId(), ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while unbanning user {UserId} by admin {AdminId}", 
                userId, RequesterId());
            return StatusCode(500, new { message = "Failed to unban user." });
        }
    }

    /// <summary>
    /// Получить список забаненных пользователей
    /// </summary>
    /// <returns></returns>
    [ResponseCache(
    Duration = 3600, 
    Location = ResponseCacheLocation.Client, 
    VaryByQueryKeys = new[] { "page", "pageSize" })]
    [HttpGet("users/banned")]
    public async Task<IActionResult> GetUserBanned(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Admin {AdminId} requested banned users list. Page: {Page}, PageSize: {PageSize}", 
            RequesterId(), page, pageSize);

        try
        {
            var command = new GetBannedUserQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid pagination parameters by admin {AdminId}: Page={Page}, PageSize={PageSize}", 
                RequesterId(), page, pageSize);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching banned users by admin {AdminId}", RequesterId());
            return StatusCode(500, new { message = "Failed to retrieve banned users." });
        }
    }
}