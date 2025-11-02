using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;
    private readonly ILogger<CreateCommentCommandHandler> _logger;
    
    public CreateCommentCommandHandler(
        IPostRepository postRepository, 
        ICommentRepository commentRepository, 
        IUserService userService,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to create a comment on post {PostId}", 
            request.AuthorId, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            _logger.LogWarning("Comment creation failed: post not found. PostId: {PostId}, UserId: {UserId}", 
                request.PostId, request.AuthorId);
            throw new NotFoundException("Post not found");
        }

        var user = await _userService.GetByIdAsync(request.AuthorId, cancellationToken);
        if (user == null)
        {
            _logger.LogError("Comment creation failed: user not found. UserId: {UserId}", request.AuthorId);
            throw new NotFoundException("User not found");
        }

        // Проверка прав: только доверенные роли могут комментировать
        var isTrusted = await _userService.IsInRoleAsync(user.Id, RoleConstants.User) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.ContentEditor) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Moderator) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Admin);

        if (!isTrusted)
        {
            _logger.LogWarning("Access denied: user {UserId} does not have permission to comment", request.AuthorId);
            throw new UnauthorizedAccessException("You do not have permission to comment.");
        }

        // Проверка родительского комментария (если ответ)
        if (request.ParentCommentId.HasValue)
        {
            var parentId = request.ParentCommentId.Value;
            
            _logger.LogDebug("Validating parent comment {ParentCommentId} for reply", parentId);
            
            var parentComment = await _commentRepository.GetByIdAsync(parentId, cancellationToken);
            if (parentComment == null)
            {
                _logger.LogWarning("Invalid parent comment: comment {ParentId} not found", parentId);
                throw new NotFoundException("Parent comment not found");
            }
            
            if (parentComment.IsDeleted)
            {
                _logger.LogWarning("Attempt to reply to deleted comment {ParentId}", parentId);
                throw new NotFoundException("Cannot reply to a deleted comment");
            }
            
            if (parentComment.PostId != request.PostId)
            {
                _logger.LogWarning("Parent comment {ParentId} belongs to another post ({ParentPostId})", 
                    parentId, parentComment.PostId);
                throw new ArgumentException("Parent comment does not belong to the same post");
            }

            _logger.LogDebug("Reply validated: comment {CommentId} → parent {ParentId}", request.PostId, parentId);
        }

        try
        {
            var newComment = new Comment(
                content: request.Content,
                postId: request.PostId,
                authorId: user.Id,
                parentCommentId: request.ParentCommentId);

            _logger.LogDebug("Comment entity created in memory with ID {TempId}, AuthorId: {AuthorId}", 
                newComment.Id, newComment.AuthorId);

            await _commentRepository.AddAsync(newComment, cancellationToken);

            _logger.LogInformation(
                "Comment {CommentId} created successfully on post {PostId}. ReplyTo: {ParentId}", 
                newComment.Id, request.PostId, request.ParentCommentId);

            return newComment.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save comment for post {PostId} by user {UserId}", 
                request.PostId, request.AuthorId);
            throw;
        }
    }
}