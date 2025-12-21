using ContentFlow.Application.DTOs.UserProfileDTOs;
using MediatR;

namespace ContentFlow.Application.Functions.UserProfile.Queries;

public record GetDetailUserprofileQuery(int RequesterId, int UserProfileId) : IRequest<UserProfileDto>;