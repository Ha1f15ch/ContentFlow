using ContentFlow.Application.DTOs.UserProfileDTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ContentFlow.Application.Functions.UserProfile.Commands;

public record UpdateUserAvatarCommand(
    int UserId,
    IFormFile AvatarFile) : IRequest<UserProfileDto>;