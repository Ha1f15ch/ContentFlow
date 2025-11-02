using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    private readonly ILogger<DeletePostCommandHandler> _logger;
    
    public DeletePostCommandHandler(
        IPostRepository postRepository, 
        IUserService userService,
        ILogger<DeletePostCommandHandler> logger)
    {
        _postRepository = postRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to delete post {PostId}", 
            request.UserInitiator, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Delete failed: post not found. PostId: {PostId}, RequestingUser: {UserId}", 
                request.PostId, request.UserInitiator);
            throw new NotFoundException($"Post with ID {request.PostId} was not found.");
        }

        // Проверка прав: Admin/Moderator или автор
        var isModeratorOrAdmin = await _userService.IsInRoleAsync(request.UserInitiator, RoleConstants.Admin) ||
                                 await _userService.IsInRoleAsync(request.UserInitiator, RoleConstants.Moderator);

        if (!isModeratorOrAdmin && post.AuthorId != request.UserInitiator)
        {
            _logger.LogWarning(
                "Access denied: user {UserId} tried to delete post {PostId} they do not own", 
                request.UserInitiator, request.PostId);
            throw new UnauthorizedAccessException("You don't have permission to delete this post.");
        }

        try
        {
            _logger.LogDebug("Marking post {PostId} as deleted", post.Id);
            post.MarkAsDeleted();
            
            await _postRepository.UpdateAsync(post, cancellationToken);
            
            _logger.LogInformation(
                "Post {PostId} successfully marked as deleted. Title: '{Title}', DeletedBy: {UserId}", 
                post.Id, post.Title, request.UserInitiator);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete post {PostId}. Unexpected error occurred.", request.PostId);
            throw;
        }
    }
}