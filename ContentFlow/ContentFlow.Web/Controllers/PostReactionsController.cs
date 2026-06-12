using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Post.Commands;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/reaction")]
public class PostReactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostReactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ReactionResultDto>> SetReaction(
        int postId,
        [FromBody] SetReactionRequest request,
        CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();

        var result = await _mediator.Send(
            new SetPostReactionCommand(postId, userId, request.ReactionType),
            ct);

        return Ok(result);
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<ReactionResultDto>> RemoveReaction(
        int postId,
        CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();

        var result = await _mediator.Send(
            new RemovePostReactionCommand(postId, userId),
            ct);

        return Ok(result);
    }
}
