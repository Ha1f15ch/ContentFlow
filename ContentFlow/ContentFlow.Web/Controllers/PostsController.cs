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
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IMediator mediator,
        ILogger<PostsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<PostDto>>> GetPostsAsync([FromQuery] GetPostsQuery query)
    {
        var authorId = User.GetUserId();
        _logger.LogInformation("UserId {AuthorId} try to get posts.", authorId);
        
        var request = query with{CurrentUserId = authorId};
        
        var result = await _mediator.Send(request);
        _logger.LogInformation("Result: {ItemsCount} posts.", result.Items.Count);
        
        return Ok(result);
    }

    [Authorize(Policy = "CanEditContent")]
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var authorId = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {AuthorId} try to create new post.", authorId);
        
        var command = new CreatePostCommand(
            Title: request.Title,
            Content: request.Content,
            AuthorId: authorId);
        
        var postId = await _mediator.Send(command);
        _logger.LogInformation("Post {PostId} created.", postId);
        
        return CreatedAtAction(
            nameof(GetPostById),
            new { id = postId },
            new { id = postId });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _mediator.Send(new GetPostByIdQuery(id));
        _logger.LogInformation("Get Post by id = {PostId}.", post.Id);
        
        return Ok(post);
    }

    [Authorize(Policy = "CanEditContent")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePostById(int id, [FromBody] UpdatePostModel request)
    {
        var authorId = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {AuthorId} try to update post.", authorId);
        
        var command = new UpdatePostCommand(
            PostId: id,
            Title: request.Title,
            Content: request.Content,
            TagIds: request.TagIds,
            AuthorId: authorId);
        
        await _mediator.Send(command);
        _logger.LogInformation("Post with id = {Id} has been updated.", id);
        
        return Ok();
    }

    [Authorize(Policy = "CanDeleteContent")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePostById(int id)
    {
        var userInitiator = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {UserInitiator} try to delete post.", userInitiator);
        
        var command = new DeletePostCommand(UserInitiator: userInitiator, PostId: id);
        
        await _mediator.Send(command);
        _logger.LogInformation("Post with id = {Id} has been deleted.", id);
        
        return Ok();
    }
    
    [Authorize(Policy = "CanPublish")]
    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> PublishPost(int id)
    {
        var userInitiator = User.GetAuthenticatedUserId();
        _logger.LogInformation("UserId {UserInitiator} try to publish post.", userInitiator);
        
        var command = new PublishPostCommand(PostId: id, UserId: userInitiator);

        var result = await _mediator.Send(command);
        _logger.LogInformation("Result published post = {Result}.", result);
        
        return Ok(result);
    }
}