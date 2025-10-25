using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Unit>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository  _postRepository;
    private readonly IUserService _userService;
    
    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
    }

    /// <summary>
    /// Обработка команды удаления комментария
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new NotFoundException($"Post with id {request.PostId} not found");
        
        var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
            throw new NotFoundException($"Comment {request.CommentId} not found");
        
        if(post.Id != comment.PostId)
            throw new NotFoundException($"Choose correct post with comment ID {request.CommentId}.");
        
        if (!await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Moderator) &&
            !await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Admin))
        {
            if (comment.AuthorId != request.AuthorId)
                throw new UnauthorizedAccessException("You can only delete your own comments.");
        }

        comment.Delete();
        await _commentRepository.DeleteAsync(comment, cancellationToken);
        
        return Unit.Value;
    }
}