using MediatR;

namespace ContentFlow.Application.Functions.Comments.Commands;

public record CreateCommentCommand() : IRequest<int>;