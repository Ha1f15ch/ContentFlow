using ContentFlow.Application.DTOs;

namespace ContentFlow.Application.Interfaces.Comment;

public interface IPostCommentsService
{
    public List<CommentDto> BuildCommentsTree(List<Domain.Entities.Comment> comments, Dictionary<int, string> userNames);
}