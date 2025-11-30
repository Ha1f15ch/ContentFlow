using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.Functions.UserProfile.Commands;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Resources;

namespace ContentFlow.Application.Functions.UserProfile.Handlers;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, CommonResult>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteUserProfileCommandHandler> _logger;
    
    public DeleteUserProfileCommandHandler(
        IUserProfileRepository userProfileRepository,
        IMapper mapper,
        ILogger<DeleteUserProfileCommandHandler> logger,
        IUserService userService)
    {
        _userProfileRepository = userProfileRepository;
        _mapper = mapper;
        _logger = logger;
        _userService = userService;
    }

    public async Task<CommonResult> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Start processing DeleteUserProfileCommand");

            var user = await _userService.GetByIdAsync(request.UserId, cancellationToken);
            var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                              ?? throw new NotFoundException($"UserProfile for user ID {request.UserId} not found");

            // Mark deleted
            userProfile.MarkDeleted();
            
            await _userProfileRepository.UpdateAsync(userProfile, cancellationToken);
            
            _logger.LogInformation("UserProfile marked as deleted for user ID: {UserId}", request.UserId);
            
            await _userService.DeleteUserAsync(user.Id, cancellationToken);
            
            return new CommonResult() { IsSuccess = true };
        }
        catch (Exception e)
        {
            _logger.LogError("Error in delete user profile process: {errorMessage}", e.Message);
            return new CommonResult()
            {
                IsSuccess = false,
                ErrorMessage = e.Message
            };
        }
    }
}