using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Interfaces.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(
        ILogger<DeleteCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}