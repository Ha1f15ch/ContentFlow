using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Application.Exceptions;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    
    public UpdatePostCommandHandler(
        IPostRepository  postRepository,
        IUserService userService)
    {
        _postRepository = postRepository;
        _userService = userService;
    }

    public async Task Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            throw new NotFoundException($"Post {request.PostId} not found");
        }

        if (!await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Admin) &&
            !await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Moderator))
        {
            if (post.AuthorId != request.AuthorId)
            {
                throw new UnauthorizedAccessException("No access for edit this post");
            }
        }
        
        post.UpdateContent(request.Content);
        post.SetCategory(request.CategoryId);
        post.SetSlug(request.Title);
        
        await _postRepository.UpdateAsync(post, cancellationToken);
    }
}