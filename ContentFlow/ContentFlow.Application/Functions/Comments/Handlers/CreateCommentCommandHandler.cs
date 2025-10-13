using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, int>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserService _userService;
    
    public CreateCommentCommandHandler(
        IPostRepository postRepository, 
        ICommentRepository commentRepository, 
        IUserService userService)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userService = userService;
    }

    public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            throw new NotFoundException("Post not found");
        }

        var user = await _userService.GetByIdAsync(request.AuthorId, cancellationToken);

        if (await _userService.IsInRoleAsync(user.Id, RoleConstants.User))
        {
            var newComment = new Comment
            (
                content: request.Content,
                postId: post.Id,
                authorId: user.Id,
                parentCommentId: null
            );
            
            await _commentRepository.AddAsync(newComment, cancellationToken);
            
            return newComment.Id;
        }
        
        return 0;
    }
}