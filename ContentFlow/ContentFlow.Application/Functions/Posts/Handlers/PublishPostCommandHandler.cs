using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Functions.Posts.Events;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, bool>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<PublishPostCommandHandler> _logger;
    
    public PublishPostCommandHandler(
        IPostRepository postRepository,
        IUserProfileRepository userProfileRepository,
        IUserService userService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<PublishPostCommandHandler> logger)
    {
        _postRepository = postRepository;
        _userProfileRepository = userProfileRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} requested to publish post {PostId}", 
            request.UserId, request.PostId);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Publish failed: post not found. PostId: {PostId}, RequestingUser: {UserId}", 
                request.PostId, request.UserId);
            return false;
        }

        // Проверка прав: Admin/Moderator или автор
        var isModeratorOrAdmin = await _userService.IsInRoleAsync(request.UserId, RoleConstants.Admin) ||
                                 await _userService.IsInRoleAsync(request.UserId, RoleConstants.Moderator);

        if (!isModeratorOrAdmin && post.AuthorId != request.UserId)
        {
            _logger.LogWarning(
                "Access denied: user {UserId} tried to publish post {PostId} they do not own", 
                request.UserId, request.PostId);
            return false;
        }
        
        var authorProfile = await _userProfileRepository.GetByUserIdAsync(post.AuthorId, cancellationToken);
        
        try
        {
            post.Publish();
            await _postRepository.UpdateAsync(post, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Post {PostId} published successfully. Title: '{Title}', PublishedBy: {UserId}", 
                post.Id, post.Title, request.UserId);
            
            // При публикации поста создаем событие, рассылаем его подписчикам
            if (authorProfile != null)
            {
                await _mediator.Publish(new PostPublishedNotification(post.Id, authorProfile.Id, DateTime.UtcNow), cancellationToken);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish post {PostId}. Unexpected error occurred.", request.PostId);
            return false;
        }
    }
}