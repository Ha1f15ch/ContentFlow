using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Categories.Queries;

public record GetCategoriesQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResult<CategoryDto>>;