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
        _logger.LogInformation("Updating category {CategoryId} to name '{Name}'", request.CategoryId, request.Name);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        var existing = await _categoryRepository.GetCategoryByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing != null && existing.Id != category.Id)
            throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");

        category.Update(request.Name, request.Description);

        try
        {
            await _categoryRepository.UpdateAsync(category, cancellationToken);
            _logger.LogInformation("Category updated successfully. Id: {CategoryId}", category.Id);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category in database");
            throw;
        }
    }
}