using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record ConfirmEmailCommand(
    string Email,
    string Token) : IRequest<AuthResult>;