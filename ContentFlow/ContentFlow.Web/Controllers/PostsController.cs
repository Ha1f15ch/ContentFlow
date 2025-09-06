using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Functions.Posts.Queries;
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
        var  result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
    {
        var postId = await _mediator.Send(command);
        
        return CreatedAtAction(
            nameof(GetPostById),
            new { id = postId },
            new { id = postId });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetPostById(int id)
    {
        var post = await _mediator.Send(new GetPostByIdQuery(id));
        return Ok(post);
    }
}