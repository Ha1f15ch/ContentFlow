using MediatR;

namespace ContentFlow.Application.Functions.Categories.Commands;

public record UpdateCategoryCommand(int CategoryId, int UserId, string Name, string Slug, string? Description) : IRequest<Unit>;