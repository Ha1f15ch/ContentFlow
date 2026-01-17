namespace ContentFlow.Application.Common;

public enum PostSortBy
{
    CreatedAt,
    PublishedAt,
    Title,
    CommentCount
}

public enum SortDirection
{
    Asc,
    Desc
}

public record SortOptions(
    PostSortBy SortBy = PostSortBy.CreatedAt,
    SortDirection Direction = SortDirection.Desc);