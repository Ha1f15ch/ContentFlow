using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Categories.Queries;

public class GetCategoriesQuery() : IRequest<PaginatedResult<CategoryDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}