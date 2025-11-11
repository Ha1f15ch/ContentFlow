using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentFlow.Web.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TagController> _logger;

    public CategoryController(
        IMediator mediator,
        ILogger<TagController> logger)
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
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Получить категорию по id категории
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet("{categoryId:int}")]
    [Authorize(Policy = "AdministrationDictionary")]
    public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
    {
        throw new System.NotImplementedException();
    }
}