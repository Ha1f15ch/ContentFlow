using AutoMapper;
using ContentFlow.Application.Common;
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
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
        {
            return null; // нужно создать отдельно типы данных включающий в себя типы (комментарий, пост и так далее)
        } // в этом типе данных добавить поле результат успешный ? Текст сообщения

        var comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
        {
            return null; // в результате выводим false + текст ошибки, что-то не найдено  
        }

        var authorUpdateComment = _userService.GetByIdAsync(request.AuthorId, cancellationToken);
        if (authorUpdateComment == null)
        {
            return null; // В результат выводим ошибку + false в результате 
        }

        var userRoleExist = await _userService.IsInRoleAsync(authorUpdateComment.Id, RoleConstants.ContentEditor);
        if (!userRoleExist)
        {
            return null; // выводим снова false + текст ошибки в ответном сообщении
        }
        
        // запись изменения комментария
        return new CommentDto(request.CommentId, request.NewCommentText, "stumb", DateTime.Now, new List<CommentDto>(), request.CommentId);
    }
}