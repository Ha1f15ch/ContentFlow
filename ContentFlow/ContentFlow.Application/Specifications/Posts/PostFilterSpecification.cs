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
            var search = _postFilter.Search.Trim();

            query = query.Where(p =>
                p.Title.Contains(search) ||
                p.Content.Contains(search));
        }
        
        if (_postFilter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == _postFilter.CategoryId.Value);
        }

        if (_postFilter.Status.HasValue)
        {
            query = query.Where(p => p.Status == _postFilter.Status.Value);
        }

        if (_postFilter.AuthorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == _postFilter.AuthorId.Value);
        }

        if (_postFilter.CreatedFrom.HasValue)
        {
            var createdFrom = _postFilter.CreatedFrom.Value.Date;
            query = query.Where(p => p.CreatedAt >= createdFrom);
        }

        return query;
    }
}