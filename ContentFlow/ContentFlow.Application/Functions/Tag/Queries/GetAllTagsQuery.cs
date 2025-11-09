using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Tag.Queries;

public record GetAllTagsQuery(int UserIdRequester) : IRequest<List<TagDto>>;