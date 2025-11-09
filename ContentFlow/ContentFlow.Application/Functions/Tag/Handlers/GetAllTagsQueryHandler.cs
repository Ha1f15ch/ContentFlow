using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Tag.Queries;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Tag;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Tag.Handlers;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, List<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetAllTagsQueryHandler> _logger;

    public GetAllTagsQueryHandler(
        ITagRepository tagRepository,
        IPostRepository postRepository,
        ILogger<GetAllTagsQueryHandler> logger)
    {
        _tagRepository = tagRepository;
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        return new List<TagDto>();
    }
}