using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Administration.Queries;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Administration.Handlers;

public class GetBannedUserQueryHandler : IRequestHandler<GetBannedUserQuery, PaginatedResult<UserDto>>
{
    private readonly IUserService _userService;
    private readonly ILogger<GetBannedUserQueryHandler> _logger;

    public GetBannedUserQueryHandler(
        IUserService userService,
        ILogger<GetBannedUserQueryHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<PaginatedResult<UserDto>> Handle(GetBannedUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Admin requested list of banned users. Page: {Page}, PageSize: {PageSize}",
            request.Page, request.PageSize);

        try
        {
            var result = await _userService.GetBannedUsersAsync(request.Page, request.PageSize, cancellationToken);
            
            _logger.LogDebug(
                "Retrieved {ItemCount} banned users out of {TotalCount}. Page {Page}/{TotalPages}",
                result.Items.Count, result.TotalCount, result.Page, result.TotalPages);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve banned users list");
            throw;
        }
    }
}