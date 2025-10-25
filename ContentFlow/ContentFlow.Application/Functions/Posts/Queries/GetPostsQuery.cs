using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Enums;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Queries;

public record GetPostsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    int? CategoryId = null,
    PostStatus? Status = null,
    int? CurrentUserId = null
    ) : IRequest<PaginatedResult<PostDto>>;