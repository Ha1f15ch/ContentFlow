using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Commands;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository  _postRepository;
    private readonly IUserService _userService;
    
    
    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        
    }
}