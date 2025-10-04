using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    
    public DeletePostCommandHandler(
        IPostRepository postRepository, 
        IUserService userService)
    {
        _postRepository = postRepository;
        _userService = userService;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            throw new NotFoundException($"Post with id {request.PostId} not found");
        }

        if (!await _userService.IsInRoleAsync(request.UserInitiator, RoleConstants.Admin) &&
            !await _userService.IsInRoleAsync(request.UserInitiator, RoleConstants.Moderator))
        {
            if (post.AuthorId != request.UserInitiator)
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this post");
            }
        }
        
        post.MarkAsDeleted();
        
        await _postRepository.UpdateAsync(post, cancellationToken);
    }
}