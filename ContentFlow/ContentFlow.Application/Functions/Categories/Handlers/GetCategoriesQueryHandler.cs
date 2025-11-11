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
        throw new System.NotImplementedException();
    }
}