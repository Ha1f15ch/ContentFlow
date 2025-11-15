using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Categories.Queries;
using ContentFlow.Application.Interfaces.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>
{
    private readonly ILogger<GetCategoriesQueryHandler> _logger;
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(
        ILogger<GetCategoriesQueryHandler> logger,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching categories. Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

        var result = await _categoryRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = result.Items.Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description ?? "")).ToList();

        return new PaginatedResult<CategoryDto>(dtos, result.TotalCount, request.Page, request.PageSize);
    }
}