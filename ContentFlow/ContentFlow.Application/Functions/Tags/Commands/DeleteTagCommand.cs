using MediatR;

namespace ContentFlow.Application.Functions.Tags.Commands;

public record DeleteTagCommand(int TagId, int UserId) : IRequest<Unit>;