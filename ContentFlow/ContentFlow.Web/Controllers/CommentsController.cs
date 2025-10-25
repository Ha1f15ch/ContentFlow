using System.Security.Claims;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Functions.Comments.Queries;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetComments(int postId)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new GetCommentsByPostIdQuery(PostId: postId, UserId: userId);
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> CreateComment(int postId, [FromBody] CreateCommentRequest createCommentToCreate)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new CreateCommentCommand(
            PostId: postId, 
            AuthorId: userId, 
            Content: createCommentToCreate.Content,
            ParentCommentId: createCommentToCreate.ParentCommentId);
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetComments), new { id = result }, new {id = result});
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { message = exception.Message });
        }
        
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> UpdateComment(int postId, int id, [FromBody] UpdateCommentRequest updateCommentRequest)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new UpdateCommentCommand(
            id, 
            postId, 
            updateCommentRequest.NewCommentText, 
            userId);
        
        try
        {
            await _mediator.Send(command);
            return NoContent(); // 204 - успешно обновлено
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> DeleteComment(int postId, int id)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new DeleteCommentCommand(id, postId, userId);

        try
        {
            var result = await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}