using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Comment.Commands;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/comments/{commentId:int}/reaction")]
public class CommentReactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentReactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ReactionResultDto>> SetReaction(
        int commentId,
        [FromBody] SetReactionRequest request,
        CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();

        var result = await _mediator.Send(
            new SetCommentReactionCommand(commentId, userId, request.ReactionType),
            ct);

        return Ok(result);
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<ReactionResultDto>> RemoveReaction(
        int commentId,
        CancellationToken ct)
    {
        var userId = User.GetAuthenticatedUserId();

        var result = await _mediator.Send(
            new RemoveCommentReactionCommand(commentId, userId),
            ct);

        return Ok(result);
    }
}
