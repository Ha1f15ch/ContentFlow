using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Comments.Queries;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Handlers;

public class GetCommentsByPostIdQueryHandler : IRequestHandler<GetCommentsByPostIdQuery, List<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    private readonly IPostCommentsService _postCommentsService;
    
    public GetCommentsByPostIdQueryHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService,
        IPostCommentsService postCommentsService)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userService = userService;
        _postCommentsService = postCommentsService;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsByPostIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post == null)
        {
            throw new NotFoundException($"Post with id: {request.PostId} not found");
        }

        var postComments = await _commentRepository.GetApprovedByPostIdAsync(request.PostId, cancellationToken);

        var commentsAuthorName = postComments.Select(c => c.AuthorId).Distinct();
        var users = await _userService.GetByIdsAsync(commentsAuthorName.ToList(), cancellationToken);
        var userDict = users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());

        return _postCommentsService.BuildCommentsTree(postComments, userDict);
    }
}