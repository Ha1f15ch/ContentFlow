using ContentFlow.Application.Common;
using ContentFlow.Application.Specifications.Abstractions;
using ContentFlow.Domain.Entities;

namespace ContentFlow.Application.Specifications.Posts;

public class PostSortingSpecification : IQuerySpecification<Post>
{
    private readonly SortOptions _sort;
    
    public PostSortingSpecification(SortOptions? sort)
    {
        _sort = sort ?? new SortOptions();
    }
    
    public IQueryable<Post> Apply(IQueryable<Post> query) // Опционально, потом, реализовать через фабрику
    {
        return (_sort.SortBy, _sort.Direction) switch
        {
            (PostSortBy.CreatedAt, SortDirection.Desc)
                => query.OrderByDescending(p => p.CreatedAt),

            (PostSortBy.CreatedAt, SortDirection.Asc)
                => query.OrderBy(p => p.CreatedAt),

            (PostSortBy.PublishedAt, SortDirection.Desc)
                => query.OrderByDescending(p => p.PublishedAt),

            (PostSortBy.PublishedAt, SortDirection.Asc)
                => query.OrderBy(p => p.PublishedAt),

            (PostSortBy.Title, SortDirection.Desc)
                => query.OrderByDescending(p => p.Title),

            (PostSortBy.Title, SortDirection.Asc)
                => query.OrderBy(p => p.Title),

            _ => query.OrderByDescending(p => p.CreatedAt)
        };
    }
}