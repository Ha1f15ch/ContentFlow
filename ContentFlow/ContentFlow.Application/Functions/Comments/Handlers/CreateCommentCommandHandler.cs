using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using MediatR;
using MediatR.Pipeline;

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
        
        var isTrusted = await _userService.IsInRoleAsync(user.Id, RoleConstants.User) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.ContentEditor) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Moderator) ||
                        await _userService.IsInRoleAsync(user.Id, RoleConstants.Admin);
        
        if (!isTrusted)
            throw new UnauthorizedAccessException("You do not have permission to comment.");

        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value, cancellationToken);
            if (parentComment == null || parentComment.IsDeleted || parentComment.PostId != request.PostId)
                throw new NotFoundException("Invalid parent comment");
        }
        
        var newComment = new Comment
        (
            content: request.Content,
            postId: request.PostId,
            authorId: user.Id,
            parentCommentId: request.ParentCommentId
            );
            
        await _commentRepository.AddAsync(newComment, cancellationToken);
            
        return newComment.Id;
    }
}