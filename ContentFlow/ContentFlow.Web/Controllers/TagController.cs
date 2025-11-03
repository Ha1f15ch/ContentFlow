using ContentFlow.Application.DTOs.TagDtos;
using ContentFlow.Application.Functions.Tag.Queries;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/tags/")]
public class TagController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TagController> _logger;

    public TagController(
        IMediator mediator,
        ILogger<TagController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTagsAsync()
    {
        var userInitiator = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {UserInitiator} try to publish post.", userInitiator);
        
        var commandQuery = new GetAllTagsQuery(userInitiator);
        var result = await _mediator.Send(commandQuery);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTagAsync([FromBody] TagRequest tag)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTagAsync(int id, [FromBody] TagUpdateRequest tag)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTagAsync(int id)
    {
        throw new NotImplementedException();
    }
}