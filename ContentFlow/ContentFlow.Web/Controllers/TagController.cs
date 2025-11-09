using ContentFlow.Application.DTOs.TagDTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Tags.Commands;
using ContentFlow.Application.Functions.Tags.Queries;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/tags")]
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
    
    /// <summary>
    /// Получить все теги (публично)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(
        Duration = 3600, 
        Location = ResponseCacheLocation.Client, 
        VaryByQueryKeys = new[] { "page", "pageSize" })]
    public async Task<IActionResult> GetTagsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching tags. Page: {Page}, PageSize: {PageSize}", page, pageSize);
        try
        {
            var query = new GetTagsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            
            _logger.LogInformation("Returning {ItemCount} tags out of {TotalCount}. Page {Page}/{TotalPages}",
                result.Items.Count, result.TotalCount, result.Page, result.TotalPages);
            
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message, "Tags not found.");
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message, "Operation process was not successful.");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message, "Internal Server Error.");
            return StatusCode(500, new { message = exception.Message });
        }
    }

    /// <summary>
    /// Получить тэг по id тега
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpGet("{tagId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> GetTagByIdAsync(int tagId)
    {
        _logger.LogInformation("Fetching tag by id = {tagId}.", tagId);
        
        var command = new GetTagsByIdQuery(tagId);

        try
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Tag is founded. TagName {tagName}", result.Name);

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Tag with ID {TagId} not found", tagId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching tag {TagId}", tagId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Создать новый тэг
    /// </summary>
    /// <param name="request"></param>
    /// <returns>int</returns>
    [HttpPost]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> CreateNewTagAsync([FromBody] CreateTagRequest request)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Request to create new tag by name = {tagName}. UserId requested command = {userId}", request.Name, userIdByClaims);
        
        var command = new CreateTagCommand(request.Name, userIdByClaims);

        try
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Tag crated with name {tagName}. TagId = {tagId}", request.Name, result);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) 
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during tag creation");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Изменить тэг
    /// </summary>
    /// <param name="tagId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{tagId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> UpdateTagAsync(int tagId, [FromBody] UpdateTagRequest request)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Request to update tag by id = {tagId}. UserId requested command = {userId}", tagId, userIdByClaims);
        
        var command = new UpdateTagCommand(request.Name, request.Slug, tagId, userIdByClaims);

        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Tag is updated. The new TagName {tagName}", request.Name);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Tag with ID {TagId} not found", tagId);
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument or conflict during tag update");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during tag update. TagId: {TagId}", tagId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Удалить тэг
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpDelete("{tagId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> DeleteTagAsync(int tagId)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Request to delete tag by id = {tagId}. UserId requested command = {userId}", tagId, userIdByClaims);
        
        var command = new DeleteTagCommand(tagId, userIdByClaims);

        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Tag is deleted by userId {userid}", userIdByClaims);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex.Message, "Tag not found.");
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogError("User unauthorized.");
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex.Message, "Operation process was not successful.");
            return BadRequest(new { message = ex.Message });
        }
    }
}