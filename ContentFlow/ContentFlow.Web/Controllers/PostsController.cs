using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Functions.Posts.Queries;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/posts/")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PostDto>>> GetPostsAsync([FromQuery] GetPostsQuery query)
    {
        var authorId = User.GetUserId();
        
        var request = query with{CurrentUserId = authorId};
        
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [Authorize(Policy = "CanEditContent")]
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var authorId = User.GetAuthenticatedUserId();
        
        var command = new CreatePostCommand(
            Title: request.Title,
            Content: request.Content,
            AuthorId: authorId,
            CategoryId: request.CategoryId);
        
        var postId = await _mediator.Send(command);
        
        return CreatedAtAction(
            nameof(GetPostById),
            new { id = postId },
            new { id = postId });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _mediator.Send(new GetPostByIdQuery(id));
        return Ok(post);
    }

    [Authorize(Policy = "CanEditContent")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePostById(int id, [FromBody] UpdatePostModel request)
    {
        var authorId = User.GetAuthenticatedUserId();
        
        var command = new UpdatePostCommand(
            PostId: id,
            Title: request.Title,
            Content: request.Content,
            CategoryId: request.CategoryId,
            TagIds: request.TagIds,
            AuthorId: authorId);
        
        await _mediator.Send(command);

        return Ok();
    }

    [Authorize(Policy = "CanDeleteContent")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePostById(int id)
    {
        var userInitiator = User.GetAuthenticatedUserId();
        var command = new DeletePostCommand(UserInitiator: userInitiator, PostId: id);
        
        await _mediator.Send(command);

        return Ok();
    }
    
    [Authorize(Policy = "CanPublish")]
    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> PublishPost(int id)
    {
        var userInitiator = User.GetAuthenticatedUserId();
        var command = new PublishPostCommand(PostId: id, UserId: userInitiator);

        var result = await _mediator.Send(command);
        
        return Ok(result);
    }
}