using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Administration.Queries;

public record SearchUsersQuery(string Query, int Limit) : IRequest<List<UserDto>>;