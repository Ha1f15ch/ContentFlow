using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    private readonly ILogger<CreatePostCommandHandler> _logger;
    
    public CreatePostCommandHandler(
        IPostRepository postRepository, 
        IUserService userService,
        ILogger<CreatePostCommandHandler> logger)
    {
        _postRepository = postRepository;
        _userService = userService;
        _logger = logger;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "User {UserId} started creating a new post with title: '{Title}'", 
            request.AuthorId, request.Title);

        var postAuthor = await _userService.GetByIdAsync(request.AuthorId, cancellationToken);
        
        if (postAuthor == null)
        {
            _logger.LogError("Failed to create post: author not found for ID {UserId}", request.AuthorId);
            throw new NotFoundException($"Author with ID {request.AuthorId} was not found.");
        }

        _logger.LogDebug("Author validated: {Id} {UserName} (Email: {Email})", 
            postAuthor.Id, postAuthor.UserName, postAuthor.Email);

        var post = new Post(
            title: request.Title,
            content: request.Content,
            authorId: request.AuthorId
        );

        _logger.LogDebug("Post entity created in memory with ID {TempId}, AuthorId: {AuthorId}", 
            post.Id, post.AuthorId);

        try
        {
            await _postRepository.AddAsync(post, cancellationToken);
            _logger.LogInformation("Post successfully created and saved. PostId: {PostId}, Title: '{Title}'", 
                post.Id, post.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save post to database. Title: '{Title}', AuthorId: {AuthorId}", 
                request.Title, request.AuthorId);
            throw;
        }

        return post.Id;
    }
}