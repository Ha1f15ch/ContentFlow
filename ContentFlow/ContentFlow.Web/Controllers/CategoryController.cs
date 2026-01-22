using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Functions.Categories.Queries;
using ContentFlow.Application.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(
        IMediator mediator,
        ILogger<CategoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Получить все категории (публично)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(
        Duration = 3600,
        Location = ResponseCacheLocation.Client,
        VaryByQueryKeys = new[] { "page", "pageSize" })]
    public async Task<IActionResult> GetAllCategoriesAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Fetching all categories. Page: {Page}, PageSize: {PageSize}", page, pageSize);

        try
        {
            var query = new GetCategoriesQuery(page, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch categories");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    /// <summary>
    /// Получить категорию по id категории
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet("{categoryId:int}", Name = "GetCategoryById")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
    {
        _logger.LogInformation("Fetching category by ID: {CategoryId}", categoryId);

        try
        {
            var query = new GetCategoriesByIdQuery(categoryId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with ID {CategoryId} not found", categoryId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    /// <summary>
    /// Создать новую категорию
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryCommand commandRequest)
    {
        _logger.LogInformation("Creating new category: '{Name}'", commandRequest.Name);

        try
        {
            var categoryId = await _mediator.Send(commandRequest);
            return CreatedAtRoute("GetCategoryById", new { categoryId }, new { id = categoryId });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid data during category creation");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create category");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }
    
    /// <summary>
    /// Обновить категорию
    /// </summary>
    [HttpPut("{categoryId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> UpdateCategoryAsync(int categoryId, [FromBody] UpdateCategoryCommand command)
    {
        var cmd = command with { CategoryId = categoryId };
        _logger.LogInformation("Updating category {CategoryId} to '{Name}'", categoryId, command.Name);

        try
        {
            await _mediator.Send(cmd);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with ID {CategoryId} not found", categoryId);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict during category update");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }
    
    /// <summary>
    /// Удалить категорию
    /// </summary>
    [HttpDelete("{categoryId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
    {
        var userIdByClaims = User.GetAuthenticatedUserId();
        _logger.LogInformation("Deleting category with ID: {CategoryId} by userid = {UserId}", categoryId, userIdByClaims);
        
        try
        {
            var command = new DeleteCategoryCommand(categoryId, userIdByClaims);
            
            await _mediator.Send(command);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Category with ID {CategoryId} not found", categoryId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete category {CategoryId}", categoryId);
            return StatusCode(500, new { message = "Internal server error." });
        }
    }
}