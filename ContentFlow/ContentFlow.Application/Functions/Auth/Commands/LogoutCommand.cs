using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record LogoutCommand(int UserId) :  IRequest<bool>;