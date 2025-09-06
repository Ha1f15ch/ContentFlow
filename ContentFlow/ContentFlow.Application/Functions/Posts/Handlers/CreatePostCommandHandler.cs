using ContentFlow.Application.Functions.Posts.Commands;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Handlers;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, int>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserService  _userService;
    
    public CreatePostCommandHandler(IPostRepository postRepository, IUserService userService)
    {
        _postRepository = postRepository;
        _userService = userService;
    }

    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var postAuthor = await _userService.GetByIdAsync(request.AuthorId, cancellationToken);

        var post = new Post(
            title: request.Title,
            content: request.Content,
            authorId: request.AuthorId,
            categoryId: request.CategoryId
        );
        
        await _postRepository.AddAsync(post, cancellationToken);
        
        return post.Id;
    }
}