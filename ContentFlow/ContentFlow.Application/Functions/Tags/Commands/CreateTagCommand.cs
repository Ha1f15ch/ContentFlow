using MediatR;

namespace ContentFlow.Application.Functions.Tags.Commands;

public record CreateTagCommand(string Name, int UserId) : IRequest<int>;