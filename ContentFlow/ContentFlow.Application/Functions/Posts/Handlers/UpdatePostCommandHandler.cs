using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    private readonly ILogger<UpdatePostCommandHandler> _logger;
    
    public UpdatePostCommandHandler(
        IPostRepository  postRepository,
        IUserService userService,
        ILogger<UpdatePostCommandHandler> logger)
    {
        _postRepository = postRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to update post {PostId}", 
            request.AuthorId, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Update failed: post not found. PostId: {PostId}, RequestingUser: {UserId}", 
                request.PostId, request.AuthorId);
            throw new NotFoundException($"Post {request.PostId} not found");
        }

        // Проверка прав: Admin/Moderator или автор
        var isModeratorOrAdmin = await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Admin) ||
                                 await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Moderator);

        if (!isModeratorOrAdmin && post.AuthorId != request.AuthorId)
        {
            _logger.LogWarning(
                "Access denied: user {UserId} tried to edit post {PostId} they do not own", 
                request.AuthorId, request.PostId);
            throw new UnauthorizedAccessException("You do not have permission to edit this post.");
        }

        try
        {
            _logger.LogDebug("Updating content and metadata for post {PostId}", post.Id);
            
            post.UpdateContent(request.Content);
            post.SetSlug(request.Title);
            
            await _postRepository.UpdateAsync(post, cancellationToken);
            
            _logger.LogInformation(
                "Post {PostId} updated successfully. Title: '{Title}', UpdatedBy: {UserId}", 
                post.Id, post.Title, request.AuthorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update post {PostId}. Unexpected error occurred.", request.PostId);
            throw;
        }
    }
}