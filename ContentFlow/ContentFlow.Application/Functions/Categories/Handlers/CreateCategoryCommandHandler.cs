using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Interfaces.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class CreateCategoryCommandHandler :  IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    private readonly ICategoryRepository _categoryRepository;
    
    public CreateCategoryCommandHandler(
        ILogger<CreateCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}