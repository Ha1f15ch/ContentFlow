using ContentFlow.Application.DTOs.AdminControllerDTOs;
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
}