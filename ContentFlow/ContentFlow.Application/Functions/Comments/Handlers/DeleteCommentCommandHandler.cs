using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository  _postRepository;
    private readonly IUserService _userService;
    private readonly ILogger<DeleteCommentCommandHandler> _logger;
    
    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService,
        ILogger<DeleteCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Обработка команды удаления комментария
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to delete comment {CommentId} on post {PostId}", 
            request.AuthorId, request.CommentId, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            _logger.LogWarning("Delete failed: post not found. PostId: {PostId}, CommentId: {CommentId}, RequestingUser: {UserId}", 
                request.PostId, request.CommentId, request.AuthorId);
            throw new NotFoundException($"Post with id {request.PostId} not found");
        }

        var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
        {
            _logger.LogWarning("Delete failed: comment not found. CommentId: {CommentId}, RequestingUser: {UserId}", 
                request.CommentId, request.AuthorId);
            throw new NotFoundException($"Comment {request.CommentId} not found");
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
                "Access denied: user {UserId} tried to delete comment {CommentId} they do not own", 
                request.AuthorId, request.CommentId);
            throw new UnauthorizedAccessException("You can only delete your own comments.");
        }

        try
        {
            _logger.LogDebug("Marking comment {CommentId} as deleted", comment.Id);
            comment.Delete();

            await _commentRepository.DeleteAsync(comment, cancellationToken);

            _logger.LogInformation(
                "Comment {CommentId} successfully marked as deleted. DeletedBy: {UserId}, PostId: {PostId}", 
                comment.Id, request.AuthorId, request.PostId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete comment {CommentId}. Unexpected error occurred.", request.CommentId);
            throw;
        }

        return Unit.Value;
    }
}