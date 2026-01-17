using ContentFlow.Application.Specifications.Abstractions;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Specifications.Posts;

public class PostVisibilitySpecification : IQuerySpecification<Post>
{
    private readonly int? _currentUserId;
    
    public PostVisibilitySpecification(int ? currentUserId)
    {
        _currentUserId = currentUserId;
    }
    
    public IQueryable<Post> Apply(IQueryable<Post> query)
    {
        if (!_currentUserId.HasValue)
        {
            return query.Where(p => p.Status == PostStatus.Published);
        }
        
        var userId = _currentUserId.Value;
        
        return query.Where(p =>
            p.Status == PostStatus.Published ||
            p.Status == PostStatus.Archived ||
            (p.Status == PostStatus.Draft && p.AuthorId == userId) ||
            (p.Status == PostStatus.PendingModeration && p.AuthorId == userId));
    }
}