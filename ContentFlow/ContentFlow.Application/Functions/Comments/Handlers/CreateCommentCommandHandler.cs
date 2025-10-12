using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
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

    public async Task<int> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        
    }
}