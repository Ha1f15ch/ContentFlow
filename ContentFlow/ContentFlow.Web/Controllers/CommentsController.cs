using System.Security.Claims;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Functions.Comments.Queries;
using ContentFlow.Application.Security;
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
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(
        IMediator mediator,
        ILogger<CommentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> GetComments(int postId)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Getting comments for post {PostId}", postId);
        
        var command = new GetCommentsByPostIdQuery(PostId: postId, UserId: userIdByClaims);

        try
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Comment is founded. Count = {Result}.", result.Count);

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex.Message, "Comment not found.");
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex.Message, "User unauthorized.");
            return Unauthorized(new { message = ex.Message });
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

    [HttpPost]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> CreateComment(int postId, [FromBody] CreateCommentRequest createCommentToCreate)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Creating comment for post {PostId} by userId = {userid}", postId, userIdByClaims);
        
        var command = new CreateCommentCommand(
            PostId: postId, 
            AuthorId: userIdByClaims, 
            Content: createCommentToCreate.Content,
            ParentCommentId: createCommentToCreate.ParentCommentId);
        try
        {
            var result = await _mediator.Send(command);
            _logger.LogInformation("Comment is created. Id = {Result}.", result);
            
            return CreatedAtAction(nameof(GetComments), new { postId }, new { id = result });
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex.Message, "Comment not found.");
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex.Message, "User unauthorized.");
            return Unauthorized(new { message = ex.Message });
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

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> UpdateComment(int postId, int id, [FromBody] UpdateCommentRequest updateCommentRequest)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("User id {id} who want to update comment", userIdByClaims);
        
        var command = new UpdateCommentCommand(
            id, 
            postId, 
            updateCommentRequest.NewCommentText, 
            userIdByClaims);
        
        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Comment is updated by userId {userid}", userIdByClaims);
            
            return NoContent(); // 204 - успешно обновлено
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex.Message, "Comment not found.");
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogError("User unauthorized.");
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex.Message, "Invalid argument.");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> DeleteComment(int postId, int id)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {UserId} who want to delete comment",  userIdByClaims);
        
        var command = new DeleteCommentCommand(id, postId, userIdByClaims);

        try
        {
            await _mediator.Send(command);
            _logger.LogInformation("Comment is deleted by userId {userid}", userIdByClaims);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex.Message, "Comment not found.");
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