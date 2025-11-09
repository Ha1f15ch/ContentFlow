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
            "User {UserId} requested to update tag with new name: '{Name}', slug: '{Slug}'",
            request.UserId, request.Name, request.Slug);
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Tag update failed: name is null or empty");
            throw new ArgumentException("Tag name is required", nameof(request.Name));
        }
        
        var normalizedName = request.Name.Trim();
        var slug = !string.IsNullOrWhiteSpace(request.Slug)
            ? request.Slug.Trim().ToLowerInvariant()
            : Tag.GenerateSlug(normalizedName);
        
        _logger.LogDebug("Normalized update data: Name='{NormalizedName}', Slug='{Slug}'", 
            normalizedName, slug);
        
        var existingTag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);
        
        var existingByName = await _tagRepository.GetByNameAsync(normalizedName, cancellationToken);
        if (existingByName != null && existingByName.Id != existingTag.Id)
        {
            _logger.LogWarning(
                "Tag update failed: tag with name '{Name}' already exists (ID: {ExistingId})", 
                normalizedName, existingByName.Id);
            throw new InvalidOperationException($"Tag with name '{normalizedName}' already exists.");
        }
        
        var existingBySlug = await _tagRepository.GetBySlugAsync(slug, cancellationToken);
        if (existingBySlug != null && existingBySlug.Id != existingTag.Id)
        {
            _logger.LogWarning(
                "Tag update failed: tag with slug '{Slug}' already exists (ID: {ExistingId})", 
                slug, existingBySlug.Id);
            throw new InvalidOperationException($"Tag with slug '{slug}' already exists.");
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