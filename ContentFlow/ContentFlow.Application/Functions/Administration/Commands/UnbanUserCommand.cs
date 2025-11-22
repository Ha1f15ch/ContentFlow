using MediatR;

namespace ContentFlow.Application.Functions.Administration.Commands;

public record UnbanUserCommand(int UserId, int RequesterId) : IRequest<Unit>;