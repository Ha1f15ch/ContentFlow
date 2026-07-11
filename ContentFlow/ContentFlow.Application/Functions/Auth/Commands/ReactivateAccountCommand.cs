using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record ReactivateAccountCommand(string Email, string Password, ClientMetadata Metadata) : IRequest<AuthResult>;
