using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository  _postRepository;
    private readonly IUserService _userService;
    private readonly ILogger<UpdateCommentCommandHandler> _logger;
    
    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService,
        ILogger<UpdateCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Обработка изменения комментария
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task (Unit)</returns>
    public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to update comment {CommentId} on post {PostId}", 
            request.AuthorId, request.CommentId, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            _logger.LogWarning("Update failed: post not found. PostId: {PostId}, CommentId: {CommentId}, RequestingUser: {UserId}", 
                request.PostId, request.CommentId, request.AuthorId);
            throw new NotFoundException($"Post with id {request.PostId} not found");
        }

        var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
        {
            _logger.LogWarning("Update failed: comment not found. CommentId: {CommentId}, RequestingUser: {UserId}", 
                request.CommentId, request.AuthorId);
            throw new NotFoundException($"Comment with ID {request.CommentId} not found.");
        }

        if (post.Id != comment.PostId)
        {
            _logger.LogWarning("Mismatch: comment {CommentId} does not belong to post {PostId}. RequestingUser: {UserId}", 
                request.CommentId, request.PostId, request.AuthorId);
            throw new NotFoundException($"Comment {request.CommentId} does not belong to the specified post.");
        }

        // Проверка прав: модератор/админ или автор
        var isModeratorOrAdmin = await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Moderator) ||
                                 await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Admin);

        if (!isModeratorOrAdmin && comment.AuthorId != request.AuthorId)
        {
            _logger.LogWarning(
                "Access denied: user {UserId} tried to edit comment {CommentId} they do not own", 
                request.AuthorId, request.CommentId);
            throw new UnauthorizedAccessException("You can only edit your own comments.");
        }

        try
        {
            _logger.LogDebug("Updating content for comment {CommentId}", comment.Id);
            comment.Edit(request.NewCommentText);

            await _commentRepository.UpdateAsync(comment, cancellationToken);

            _logger.LogInformation(
                "Comment {CommentId} successfully updated by user {UserId}. PostId: {PostId}", 
                comment.Id, request.AuthorId, request.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update comment {CommentId}. Unexpected error occurred.", request.CommentId);
            throw;
        }

        return Unit.Value;
    }
}