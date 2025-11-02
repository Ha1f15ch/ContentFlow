using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Services;

public class PostCommentsService : IPostCommentsService
{
    private readonly ILogger<PostCommentsService> _logger;
    
    public PostCommentsService(ILogger<PostCommentsService> logger)
    {
        _logger = logger;
    }
    
    public List<CommentDto> BuildCommentsTree(List<Comment> comments, Dictionary<int, string> userNames)
    {
        if (comments == null)
            throw new ArgumentNullException(nameof(comments));
        
        _logger.LogDebug("Building comment tree for {Count} comments", comments.Count);
        var commentsDto = comments.Select(c => new CommentDto(
            Id: c.Id,
            PostId: c.PostId,
            Content: c.Content,
            AuthorName: userNames.GetValueOrDefault(c.AuthorId, "Unknown"),
            CreatedAt: c.CreatedAt,
            Comments: new List<CommentDto>(),
            ParentCommentId: c.ParentCommentId,
            CommentStatus: c.Status.ToString()
            )).ToList();

        var commentDict = commentsDto.ToDictionary(c => c.Id);
        var rootComments = new List<CommentDto>();

        foreach (var dto in commentsDto)
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
        
        _logger.LogDebug("Comment tree built with {RootCount} root comments", rootComments.Count);
        return rootComments;
    }
}