using ContentFlow.Application.Functions.Tags.Commands;
using ContentFlow.Application.Interfaces.Tag;
using ContentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tags.Handlers;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<CreateTagCommandHandler> _logger;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        ILogger<CreateTagCommandHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to create tag with name: '{Name}'",
            request.UserId, request.Name);
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Tag creation failed: name is null or empty");
            throw new ArgumentException("Tag name is required", nameof(request.Name));
        }
        
        var normalizedName = request.Name.Trim();
        
        _logger.LogDebug("Normalized tag data: Name='{NormalizedName}'", 
            normalizedName);
        
        var existingByName = await _tagRepository.GetByNameAsync(normalizedName, cancellationToken);
        if (existingByName != null)
        {
            _logger.LogWarning("Tag creation failed: tag with name '{Name}' already exists", normalizedName);
            throw new InvalidOperationException($"Tag with name '{normalizedName}' already exists.");
        }
        
        var tag = new Tag(normalizedName);
        
        try
        {
            await _tagRepository.AddAsync(tag, cancellationToken);
            
            _logger.LogInformation(
                "Tag created successfully. TagId: {TagId}, Name: '{Name}', Slug: '{Slug}'", 
                tag.Id, tag.Name, tag.Slug);
                
            return tag.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tag to database. Name: '{Name}'", 
                normalizedName);
            throw;
        }
    }
}