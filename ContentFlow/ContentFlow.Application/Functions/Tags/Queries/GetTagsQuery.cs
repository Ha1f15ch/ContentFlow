using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Tags.Queries;

public class GetTagsQuery() : IRequest<PaginatedResult<TagDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}