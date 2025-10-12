using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Infrastructure.Services;

public class PostCommentsService : IPostCommentsService
{
    public PostCommentsService()
    {
        
    }
    
    public List<CommentDto> BuildCommentsTree(List<Comment> comments, Dictionary<int, string> userNames)
    {
        var commentsDtos = comments.Select(c => new CommentDto(
            Id: c.Id,
            Content: c.Content,
            AuthorName: userNames.GetValueOrDefault(c.AuthorId, "Unknown"),
            CreatedAt: c.CreatedAt,
            Comments: new List<CommentDto>(),
            ParentCommentId: c.ParentCommentId
            )).ToList();

        var commentDict = commentsDtos.ToDictionary(c => c.Id);
        
        var rootComments = new List<CommentDto>();

        foreach (var dto in commentsDtos)
        {
            if (dto.ParentCommentId is int parentId && commentDict.TryGetValue(parentId, out var parentDto))
            {
                parentDto.Comments.Add(dto);
            }
            else
            {
                rootComments.Add(dto);
            }
        }
        
        return rootComments;
    }
}