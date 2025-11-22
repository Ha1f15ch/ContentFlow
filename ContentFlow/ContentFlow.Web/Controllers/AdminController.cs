using ContentFlow.Application.Functions.Administration.Commands;
using ContentFlow.Application.Functions.Administration.Queries;
using ContentFlow.Application.Security;
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
        var result = await _mediator.Send(new SearchUsersQuery(query, limit));
        
        return Ok(result);
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
        await _mediator.Send(new BanUserCommand(userId, banReason, RequesterId()));
        
        return NoContent();
    }

    /// <summary>
    /// Разбанить пользователя
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPost("users/{userId:int}/unban")]
    public async Task<IActionResult> UnbanUserAsync(int userId)
    {
        var result = await _mediator.Send(new UnbanUserCommand(userId, RequesterId()));
        
        return Ok(result);
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
        var command = new GetBannedUserQuery { Page = page, PageSize = pageSize };
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
}