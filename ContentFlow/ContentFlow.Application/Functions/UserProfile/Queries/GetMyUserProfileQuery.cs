using ContentFlow.Application.DTOs.UserProfileDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.UserProfile.Queries;

public record GetMyUserProfileQuery(int UserId) : IRequest<UserProfileDto>;