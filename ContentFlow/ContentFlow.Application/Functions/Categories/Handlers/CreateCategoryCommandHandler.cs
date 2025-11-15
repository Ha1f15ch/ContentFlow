using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Interfaces.Category;
using ContentFlow.Domain.Entities;
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
        _logger.LogInformation("User requested to create category '{Name}'", request.Name);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Category name is required", nameof(request.Name));

        var existing = await _categoryRepository.GetCategoryByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");

        var category = new Category(request.Name, request.Description);

        try
        {
            await _categoryRepository.AddAsync(category, cancellationToken);
            _logger.LogInformation("Category created successfully. Id: {CategoryId}, Name: '{Name}'", category.Id, category.Name);
            return category.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save category to database");
            throw;
        }
    }
}