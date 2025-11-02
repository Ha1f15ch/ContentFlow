using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Posts.Queries;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUserService  _userService;
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<GetPostByIdQueryHandler> _logger;
    
    public GetPostByIdQueryHandler(
        IPostRepository postRepository, 
        IMapper mapper, 
        IMediator mediator, 
        IUserService userService, 
        ICommentRepository commentRepository,
        ILogger<GetPostByIdQueryHandler> logger)
    {
        _postRepository = postRepository;
        _mapper = mapper;
        _mediator = mediator;
        _userService = userService;
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching post by ID: {PostId}", request.Id);

        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (post == null)
        {
            _logger.LogWarning("Post not found for ID: {PostId}", request.Id);
            throw new NotFoundException($"Post with ID {request.Id} was not found.");
        }

        _logger.LogInformation("Post found: '{Title}' by user {AuthorId}, Status: {Status}", 
            post.Title, post.AuthorId, post.Status);

        var author = await _userService.GetByIdAsync(post.AuthorId, cancellationToken);
        if (author == null)
        {
            _logger.LogError("Author not found for post {PostId}, AuthorId: {AuthorId}", post.Id, post.AuthorId);
            throw new InvalidOperationException($"Author with ID {post.AuthorId} not found.");
        }

        var commentCount = await _commentRepository.GetCountAsync(post.Id, cancellationToken);
        _logger.LogDebug("Post {PostId} has {CommentCount} approved comments", post.Id, commentCount);

        var tagDtos = post.PostTags?.Select(pt => new TagDto(
            pt.Tag.Id,
            pt.Tag.Name,
            pt.Tag.Slug)).ToList() ?? new List<TagDto>();

        _logger.LogInformation("Successfully built PostDto for post: {PostId}", post.Id);

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
            tagDtos,
            commentCount);
    }
}