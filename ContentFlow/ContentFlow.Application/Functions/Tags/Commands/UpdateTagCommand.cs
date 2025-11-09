using MediatR;

namespace ContentFlow.Application.Functions.Tags.Commands;

public record UpdateTagCommand(string Name, string Slug, int TagId, int UserId) : IRequest<Unit>;