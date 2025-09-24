using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record LoginCommand(
    string Email,
    string Password,
    ClientMetadata Metadata) : IRequest<AuthResult>;