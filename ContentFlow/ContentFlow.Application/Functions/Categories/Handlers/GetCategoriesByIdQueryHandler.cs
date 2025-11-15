using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Categories.Queries;
using ContentFlow.Application.Interfaces.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class GetCategoriesByIdQueryHandler : IRequestHandler<GetCategoriesByIdQuery, CategoryDto>
{
    private readonly ILogger<GetCategoriesByIdQueryHandler> _logger;
    private readonly ICategoryRepository _categoryRepository;
    
    public GetCategoriesByIdQueryHandler(
        ILogger<GetCategoriesByIdQueryHandler> logger,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(GetCategoriesByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching category by ID: {CategoryId}", request.CategoryId);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        return new CategoryDto(category.Id, category.Name, category.Slug, category.Description ?? "");
    }
}