using ContentFlow.Application.DTOs.ReportDTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Reports.Commands;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/comments/{commentId:int}/report")]
public class CommentReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommentReportsController> _logger;

    public CommentReportsController(IMediator mediator, ILogger<CommentReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> ReportComment(
        int commentId,
        [FromBody] CreateReportRequest request,
        CancellationToken ct)
    {
        var reporterId = User.GetAuthenticatedUserId();
        _logger.LogInformation("User {ReporterId} reporting comment {CommentId}", reporterId, commentId);

        try
        {
            var reportId = await _mediator.Send(
                new CreateReportCommentCommand(reporterId, commentId, request.ReasonType, request.Description),
                ct);

            return Created($"/api/comments/{commentId}/report", new { id = reportId });
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create report for comment {CommentId}", commentId);
            return StatusCode(500, new { message = "Failed to submit report." });
        }
    }
}
