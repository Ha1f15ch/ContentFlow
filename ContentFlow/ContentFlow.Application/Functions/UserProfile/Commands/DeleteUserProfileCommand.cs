using ContentFlow.Application.Common;
using MediatR;

namespace ContentFlow.Application.Functions.UserProfile.Commands;

public record DeleteUserProfileCommand(int UserId) : IRequest<CommonResult>;