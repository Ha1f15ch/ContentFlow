namespace ContentFlow.Application.Common;

public class PaginatedResult<T>
{
    public List<T> Items { get; }
    public int Page {get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PaginatedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}