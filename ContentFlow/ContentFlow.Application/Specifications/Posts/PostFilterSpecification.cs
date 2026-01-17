using ContentFlow.Application.Common;
using ContentFlow.Application.Specifications.Abstractions;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Specifications.Posts;

public class PostFilterSpecification : IQuerySpecification<Post>
{
    private readonly PostFilter? _postFilter;
    
    public PostFilterSpecification(PostFilter? postFilter)
    {
        _postFilter = postFilter;
    }
    
    public IQueryable<Post> Apply(IQueryable<Post> query)
    {
        if (_postFilter is null)
            return query;
        
        if (!string.IsNullOrWhiteSpace(_postFilter.Search))
        {
            query = query.Where(p =>
                p.Title.Contains(_postFilter.Search) ||
                p.Content.Contains(_postFilter.Search));
        }
        
        if (_postFilter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == _postFilter.CategoryId);
        }

        if (_postFilter.Status.HasValue)
        {
            query = query.Where(p => p.Status == _postFilter.Status);
        }

        return query;
    }
}