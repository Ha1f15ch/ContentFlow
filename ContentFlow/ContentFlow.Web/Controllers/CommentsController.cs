using System.Security.Claims;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Functions.Comments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public async Task<IActionResult> CreateComment(int postId, [FromBody] CommentDtoToCreate commentToCreate)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new CreateCommentCommand(
            PostId: postId, 
            AuthorId: userId, 
            Content: commentToCreate.Content, 
            ParentCommentId: commentToCreate.ParentCommentId);
        
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> UpdateComment(int postId, int id, [FromBody] UpdateCommentData updateCommentData)
    {
        var userIdByClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdByClaims, out var userId))
        {
            userId = 0;
        }

        var command = new UpdateCommentCommand();
        
        var result = await _mediator.Send(command);

        return Ok();
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

        var command = new DeleteCommentCommand();
        
        var result = await _mediator.Send(command);

        return Ok();
    }
}