using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Posts.Queries;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IPostRepository _postRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUserService  _userService;
    private readonly ICommentRepository _commentRepository;
    
    public GetPostByIdQueryHandler(IPostRepository postRepository,  IMapper mapper, IMediator mediator, IUserService userService, ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _mapper = mapper;
        _mediator = mediator;
        _userService = userService;
        _commentRepository = commentRepository;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
        
        var author = await _userService.GetByIdAsync(post.AuthorId, cancellationToken);
        
        var commentCount = await _commentRepository.GetCountAsync(post.Id, cancellationToken);
        
        return new PostDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Excerpt,
            post.AuthorId,
            $"{author.FirstName} {author.LastName}",
            author.AvatarUrl,
            post.Status,
            post.CreatedAt,
            post.PublishedAt,
            post.PostTags.Select(pt => new TagDto(pt.Tag.Id, pt.Tag.Name, pt.Tag.Slug)).ToList(),
            commentCount); // как вариант, в будущем данную модель можно поменять на другую, чтобы обсчитвывать эти значения на уровне БД в LINQ
    }
}