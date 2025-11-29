using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Posts.Queries;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PaginatedResult<PostDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    private readonly IUserService  _userService;
    private readonly ILogger<GetPostsQueryHandler> _logger;
    
    public GetPostsQueryHandler(
        IPostRepository postRepository,
        IMapper mapper, 
        IUserService userService,
        ILogger<GetPostsQueryHandler> logger)
    {
        _postRepository = postRepository;
        _mapper = mapper;
        _userService = userService;
        _logger = logger;
    }

    public async Task<PaginatedResult<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching posts. User: {UserId}, Page: {Page}, PageSize: {PageSize}, Search: '{Search}', CategoryId: {CategoryId}, Status: {Status}",
            request.CurrentUserId ?? 0,
            request.Page,
            request.PageSize,
            request.Search,
            request.CategoryId,
            request.Status);

        try
        {
            var posts = await _postRepository.GetAllAsync(
                page: request.Page, 
                pageSize: request.PageSize,
                search: request.Search,
                categoryId: request.CategoryId,
                status: request.Status,
                currentUserId: request.CurrentUserId,
                ct: cancellationToken);
            
            _logger.LogInformation("Found {PostCount} posts matching criteria", posts.Items.Count);

            if (!posts.Items.Any())
            {
                _logger.LogInformation("No posts matched the filter criteria");
            }

            var authorIds = posts.Items
                .Select(x => x.AuthorId)
                .Distinct()
                .ToList();
            
            if (authorIds.Any())
            {
                var authors = await _userService.GetByIdsAsync(authorIds, cancellationToken);
                var authorDict = authors.ToDictionary(x => x.Id, x => x);

                var dtos = posts.Items.Select(post =>
                {
                    var author = authorDict.GetValueOrDefault(post.AuthorId);
                    return new PostDto(
                        post.Id,
                        post.Title,
                        post.Slug,
                        post.Excerpt,
                        post.AuthorId,
                        author != null ? $"{author.UserName}".Trim() : "Unknown Author",
                        author?.AvatarUrl,
                        post.Status,
                        post.CreatedAt,
                        post.PublishedAt,
                        new List<TagDto>(),
                        post.CommentCount
                    );
                }).ToList();

                _logger.LogInformation("Successfully mapped {DtoCount} posts to DTOs", dtos.Count);
                
                return new PaginatedResult<PostDto>(dtos, posts.TotalCount, request.Page, request.PageSize);
            }
            else
            {
                _logger.LogWarning("No authors found for returned posts");
                return new PaginatedResult<PostDto>(new List<PostDto>(), posts.TotalCount, request.Page, request.PageSize);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching or mapping posts");
            throw;
        }
    }
}