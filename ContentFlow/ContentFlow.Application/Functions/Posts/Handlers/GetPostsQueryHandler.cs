using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Posts.Queries;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class GetPostsQueryHandler : IRequestHandler<GetPostsQuery, PaginatedResult<PostDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    private readonly IUserService  _userService;
    
    public GetPostsQueryHandler(IPostRepository postRepository,  IMapper mapper, IUserService userService)
    {
        _postRepository = postRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<PaginatedResult<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetAllAsync(
            page: request.Page, 
            pageSize: request.PageSize,
            search: request.Search,
            categoryId: request.CategoryId,
            status: request.Status,
            currentUserId: request.CurrentUserId,
            ct: cancellationToken);
        
        var authorIds = posts.Items
            .Select(x => x.AuthorId)
            .Distinct()
            .ToList();
        
        var authors = await _userService.GetByIdsAsync(authorIds, cancellationToken);
        var authorDict = authors.ToDictionary(x => x.Id, x => x);

        var dtos = posts.Items.Select(post =>
        {
            var author = authorDict[post.AuthorId];
            return new PostDto(
                post.Id,
                post.Title,
                post.Slug,
                post.Excerpt,
                post.AuthorId,
                $"{author.FirstName} {author.LastName}".Trim(),
                author.AvatarUrl,
                post.Status,
                post.CreatedAt,
                post.PublishedAt,
                new List<TagDto>(),
                post.CommentCount
            );
        }).ToList();
        
        return new PaginatedResult<PostDto>(dtos, posts.TotalCount, request.Page, request.PageSize);
    }
}