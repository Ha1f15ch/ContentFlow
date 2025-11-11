using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Categories.Queries;

public record GetCategoriesByIdQuery(int CategoryId) : IRequest<CategoryDto>;