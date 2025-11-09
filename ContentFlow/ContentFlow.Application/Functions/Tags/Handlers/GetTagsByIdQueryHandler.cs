using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Tags.Queries;
using ContentFlow.Application.Interfaces.Tag;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tags.Handlers;

public class GetTagsByIdQueryHandler : IRequestHandler<GetTagsByIdQuery, TagDto>
{
    private readonly ILogger<GetTagsByIdQueryHandler> _logger;
    private readonly ITagRepository _tagRepository;

    public GetTagsByIdQueryHandler(
        ITagRepository tagRepository, 
        ILogger<GetTagsByIdQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<TagDto> Handle(GetTagsByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching tag with ID: {TagId}", request.TagId);

        var tag = await _tagRepository.GetByIdAsync(request.TagId, cancellationToken);

        _logger.LogDebug("Tag found: '{Name}' (Slug: {Slug})", tag.Name, tag.Slug);

        return new TagDto(tag.Id, tag.Name, tag.Slug);
    }
}