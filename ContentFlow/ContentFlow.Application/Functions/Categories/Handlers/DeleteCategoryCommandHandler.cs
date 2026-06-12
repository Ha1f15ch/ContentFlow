using ContentFlow.Application.Functions.Categories.Commands;
using ContentFlow.Application.Interfaces.Category;
using ContentFlow.Application.Interfaces.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Categories.Handlers;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(
        ILogger<DeleteCategoryCommandHandler> logger,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", request.CategoryId);

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category deleted successfully. Id: {CategoryId}", category.Id);
        return Unit.Value;
    }
}