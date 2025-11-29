using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record RegisterCommand (
    string Email,
    string Password,
    string UserName) : IRequest<AuthResult>;