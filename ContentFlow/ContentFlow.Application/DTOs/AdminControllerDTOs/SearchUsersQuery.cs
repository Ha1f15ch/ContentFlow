using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.DTOs.AdminControllerDTOs;

public record SearchUsersQuery(string Query, int Limit) : IRequest<PaginatedResult<UserDto>>;