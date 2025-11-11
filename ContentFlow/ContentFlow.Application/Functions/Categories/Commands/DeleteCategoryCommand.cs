using MediatR;

namespace ContentFlow.Application.Functions.Categories.Commands;

public record DeleteCategoryCommand(int CategoryId, int UserId) : IRequest<Unit>;