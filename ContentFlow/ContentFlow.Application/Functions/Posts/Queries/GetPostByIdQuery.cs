using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Posts.Queries;

public record GetPostByIdQuery(int Id, int? CurrentUserId = null):  IRequest<PostDto>;