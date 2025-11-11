using MediatR;

namespace ContentFlow.Application.Functions.Categories.Commands;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<int>;