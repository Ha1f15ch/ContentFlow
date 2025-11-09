using ContentFlow.Application.Functions.Tags.Commands;
using ContentFlow.Application.Interfaces.Tag;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tags.Handlers;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Unit>
{
    private readonly ILogger<DeleteTagCommandHandler> _logger;
    private readonly ITagRepository _tagRepository;

    public DeleteTagCommandHandler(
        ILogger<DeleteTagCommandHandler> logger,
        ITagRepository tagRepository)
    {
        _logger = logger;
        _tagRepository = tagRepository;
    }

    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to delete tag with ID: {TagId}",
            request.UserId, request.TagId);

        // 1. Получаем тег из репозитория
        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        
        try
        {
            // 2. Удаляем тег
            await _tagRepository.DeleteAsync(tag, cancellationToken);
            
            _logger.LogInformation(
                "Tag deleted successfully. Id: {TagId}, Name: '{Name}'", 
                tag.Id, tag.Name);
                
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete tag from database. TagId: {TagId}", tag.Id);
            throw;
        }
    }
}