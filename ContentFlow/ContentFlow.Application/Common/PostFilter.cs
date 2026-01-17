using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Common;

public record PostFilter(
    string? Search = null,
    int? CategoryId = null,
    PostStatus? Status = null,
    SortOptions? Sort = null,
    int? AuthorId = null,
    DateTime? CreatedFrom = null);