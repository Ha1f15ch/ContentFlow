using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class PublishPostCommandHandler : IRequestHandler<PublishPostCommand, bool>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;
    
    public PublishPostCommandHandler(
        IPostRepository postRepository,
        IUserService userService)
    {
        _postRepository = postRepository;
        _userService = userService;
    }

    public async Task<bool> Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            Console.WriteLine($"Post with id = {request.PostId} not found");
            return false;
        }
        
        if (!await _userService.IsInRoleAsync(request.UserId, RoleConstants.Admin) &&
            !await _userService.IsInRoleAsync(request.UserId, RoleConstants.Moderator))
        {
            if (post.AuthorId != request.UserId)
            {
                Console.WriteLine("You don't have permission to set Publish status for this post");
                return false;
            }
        }
        
        post.Publish();
        await _postRepository.UpdateAsync(post, cancellationToken);
        return true;
    }
}