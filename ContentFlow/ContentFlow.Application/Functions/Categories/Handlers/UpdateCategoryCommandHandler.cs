using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Interfaces.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ILogger<UpdateCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}