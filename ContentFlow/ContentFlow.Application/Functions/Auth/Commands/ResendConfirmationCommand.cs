using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record ResendConfirmationCommand(string Email) :  IRequest<AuthResult>;