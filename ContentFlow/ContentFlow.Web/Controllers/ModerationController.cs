using ContentFlow.Application.DTOs.ModerationDTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Moderation.Commands;
using ContentFlow.Application.Functions.Moderation.Queries;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/moderation/cases")]
[Authorize(Policy = "CanDeleteContent")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ModerationController> _logger;

    public ModerationController(IMediator mediator, ILogger<ModerationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private int ModeratorId => User.GetAuthenticatedUserId();

    [HttpGet]
    public async Task<IActionResult> GetOpenCases(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _mediator.Send(
                new GetOpenModerationCasesQuery(ModeratorId, page, pageSize),
                ct);

            return Ok(result);
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
            _logger.LogError(ex, "Failed to load open moderation cases");
            return StatusCode(500, new { message = "Failed to load moderation cases." });
        }
    }

    [HttpGet("{caseId:int}")]
    public async Task<IActionResult> GetCaseById(int caseId, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(
                new GetModerationCaseByIdQuery(caseId, ModeratorId),
                ct);

            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load moderation case {CaseId}", caseId);
            return StatusCode(500, new { message = "Failed to load moderation case." });
        }
    }

    [HttpPost("{caseId:int}/take")]
    public async Task<IActionResult> TakeInReview(int caseId, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new TakeModerationCaseInReviewCommand(caseId, ModeratorId), ct);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to take moderation case {CaseId} in review", caseId);
            return StatusCode(500, new { message = "Failed to take case in review." });
        }
    }

    [HttpPost("{caseId:int}/resolve")]
    public async Task<IActionResult> ResolveCase(
        int caseId,
        [FromBody] ResolveModerationCaseRequest request,
        CancellationToken ct)
    {
        try
        {
            await _mediator.Send(
                new ResolveModerationCaseCommand(caseId, ModeratorId, request.Decision, request.Note),
                ct);

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
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve moderation case {CaseId}", caseId);
            return StatusCode(500, new { message = "Failed to resolve moderation case." });
        }
    }

    [HttpPost("{caseId:int}/dismiss")]
    public async Task<IActionResult> DismissCase(
        int caseId,
        [FromBody] DismissModerationCaseRequest? request,
        CancellationToken ct)
    {
        try
        {
            await _mediator.Send(
                new DismissModerationCaseCommand(caseId, ModeratorId, request?.Note),
                ct);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dismiss moderation case {CaseId}", caseId);
            return StatusCode(500, new { message = "Failed to dismiss moderation case." });
        }
    }
}
