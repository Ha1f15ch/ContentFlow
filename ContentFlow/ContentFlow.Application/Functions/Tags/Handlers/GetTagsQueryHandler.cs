using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Tags.Queries;
using ContentFlow.Application.Interfaces.Tag;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tags.Handlers;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, PaginatedResult<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<GetTagsQueryHandler> _logger;
    
    public GetTagsQueryHandler(
        ITagRepository tagRepository,
        ILogger<GetTagsQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<PaginatedResult<TagDto>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"GetTagsQueryHandler.Handle() called.");
        _logger.LogInformation("Fetching tags. Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);
        
        var result = await _tagRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        
        var dtos = result.Items.Select(t => new TagDto(t.Id, t.Name, t.Slug)).ToList();
        
        _logger.LogInformation("Returning {Count} tags", dtos.Count);
        
        return new PaginatedResult<TagDto>(dtos, result.TotalCount, request.Page, request.PageSize);
    }
}