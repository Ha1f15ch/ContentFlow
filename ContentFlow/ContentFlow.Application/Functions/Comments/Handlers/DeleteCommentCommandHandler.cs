using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
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

    public Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        
    }
}