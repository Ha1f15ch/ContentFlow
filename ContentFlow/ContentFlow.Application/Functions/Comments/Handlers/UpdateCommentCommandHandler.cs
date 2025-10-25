using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, Unit>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository  _postRepository;
    private readonly IUserService _userService;
    
    
    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
    }

    /// <summary>
    /// Обработка изменения комментария
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Task (Unit)</returns>
    public async Task<Unit> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            throw new NotFoundException($"Post with id {request.PostId} not found");
        
        var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
            throw new NotFoundException($"Comment with ID {request.CommentId} not found.");
        
        if(post.Id != comment.PostId)
            throw new NotFoundException($"Choose correct post with comment ID {request.CommentId}.");
        
        if (!await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Moderator) &&
            !await _userService.IsInRoleAsync(request.AuthorId, RoleConstants.Admin))
        {
            if (comment.AuthorId != request.AuthorId)
                throw new UnauthorizedAccessException("You can only edit your own comments.");
        }
        
        // Изменяем комментарий
        comment.Edit(request.NewCommentText);
        await _commentRepository.UpdateAsync(comment, cancellationToken);

        return Unit.Value;
    }
}