using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Administration.Queries;

public class GetBannedUserQuery : IRequest<PaginatedResult<UserDto>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}