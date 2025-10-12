using ContentFlow.Application.DTOs;
using MediatR;

namespace ContentFlow.Application.Functions.Comments.Queries;

public record GetCommentsByPostIdQuery(
    int  PostId,
    int UserId) : IRequest<List<CommentDto>>;