using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Queries;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class GetCommentsByPostIdQueryHandler : IRequestHandler<GetCommentsByPostIdQuery, List<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    private readonly IPostCommentsService _postCommentsService;
    private readonly ILogger<GetCommentsByPostIdQueryHandler> _logger;
    
    public GetCommentsByPostIdQueryHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService,
        IPostCommentsService postCommentsService,
        ILogger<GetCommentsByPostIdQueryHandler> logger)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
        _postCommentsService = postCommentsService;
        _logger = logger;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByPostIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching comments for post ID: {PostId}", request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Failed to load comments: post not found. PostId: {PostId}", request.PostId);
            throw new NotFoundException($"Post with id: {request.PostId} not found");
        }

        _logger.LogDebug("Loading approved comments for post {PostId}", request.PostId);
        var postComments = await _commentRepository.GetApprovedByPostIdAsync(request.PostId, cancellationToken);

        _logger.LogInformation("Loaded {CommentCount} approved comments for post {PostId}", postComments.Count, request.PostId);

        if (!postComments.Any())
        {
            _logger.LogInformation("No approved comments found for post {PostId}", request.PostId);
            return new List<CommentDto>();
        }

        var authorIds = postComments.Select(c => c.AuthorId).Distinct().ToList();
        _logger.LogDebug("Fetching user data for {UserCount} authors", authorIds.Count);

        var users = await _userService.GetByIdsAsync(authorIds, cancellationToken);
        var userDict = users.ToDictionary(u => u.Id, u => $"{u.UserName}".Trim());

        _logger.LogDebug("Building comment tree structure for {CommentCount} comments", postComments.Count);
        
        var commentTree = _postCommentsService.BuildCommentsTree(postComments, userDict);
        
        _logger.LogInformation("Successfully built comment tree with {RootCount} root comments for post {PostId}", 
            commentTree.Count, request.PostId);

        return commentTree;
    }
}