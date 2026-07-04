using ContentFlow.Application.DTOs.ReportDTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Reports.Commands;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/report")]
public class PostReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostReportsController> _logger;

    public PostReportsController(IMediator mediator, ILogger<PostReportsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "CanEditContent")]
    public async Task<IActionResult> ReportPost(
        int postId,
        [FromBody] CreateReportRequest request,
        CancellationToken ct)
    {
        var reporterId = User.GetAuthenticatedUserId();
        _logger.LogInformation("User {ReporterId} reporting post {PostId}", reporterId, postId);

        try
        {
            var reportId = await _mediator.Send(
                new CreateReportPostCommand(reporterId, postId, request.ReasonType, request.Description),
                ct);

            return Created($"/api/posts/{postId}/report", new { id = reportId });
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
            _logger.LogError(ex, "Failed to create report for post {PostId}", postId);
            return StatusCode(500, new { message = "Failed to submit report." });
        }
    }
}
