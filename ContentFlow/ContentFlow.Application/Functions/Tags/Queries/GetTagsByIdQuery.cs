using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Tags.Queries;

public record GetTagsByIdQuery(int TagId) : IRequest<TagDto>;