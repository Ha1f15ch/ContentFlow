using ContentFlow.Application.Functions.Tags.Commands;
using ContentFlow.Application.Interfaces.Tag;
using ContentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tags.Handlers;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Unit>
{
    private readonly ILogger<UpdateTagCommandHandler> _logger;
    private readonly ITagRepository _tagRepository;

    public UpdateTagCommandHandler(
        ITagRepository tagRepository,
        ILogger<UpdateTagCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to update tag with new name: '{Name}'",
            request.UserId, request.Name);
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Tag update failed: name is null or empty");
            throw new ArgumentException("Tag name is required", nameof(request.Name));
        }
        
        var normalizedName = request.Name.Trim();
        var slug = Tag.GenerateSlug(normalizedName);
        
        _logger.LogDebug("Normalized update data: Name='{NormalizedName}', Slug='{Slug}'", 
            normalizedName, slug);
        
        var existingTag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        
        if(existingTag == null)
            throw new InvalidOperationException($"Tag with id '{request.TagId}' does not exist.");
        
        var existingByName = await _tagRepository.GetByNameAsync(normalizedName, cancellationToken);

        if (existingByName == null)
        {
            existingTag.Rename(normalizedName);
        }
        
        if(existingByName != null && existingByName.Id != existingTag.Id)
        {
            _logger.LogWarning(
                "Tag update failed: tag with name '{Name}' already exists (ID: {ExistingId})", 
                normalizedName, existingByName.Id);
            throw new InvalidOperationException($"Tag with name '{normalizedName}' already exists.");
        }
        
        // Изменяем сущность через доменную сущность
        existingTag.Rename(normalizedName);
        
        try
        {
            await _tagRepository.UpdateAsync(existingTag, cancellationToken);
            
            _logger.LogInformation(
                "Tag updated successfully. Id: {TagId}, NewName: '{Name}', NewSlug: '{Slug}'", 
                existingTag.Id, existingTag.Name, existingTag.Slug);
                
            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save updated tag to database. Id: {TagId}", existingTag.Id);
            throw;
        }
    }
}