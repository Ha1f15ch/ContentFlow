using MediatR;

namespace ContentFlow.Application.Functions.Administration.Commands;

public record BanUserCommand(int UserId, string BanReason, int RequesterId) : IRequest<Unit>;