using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.Auth.Commands;

public record RefreshCommand(string RefreshToken,
    ClientMetadata Metadata) : IRequest<AuthResult>;