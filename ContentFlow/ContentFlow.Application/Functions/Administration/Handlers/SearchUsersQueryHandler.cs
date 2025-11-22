using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Functions.Administration.Queries;
using ContentFlow.Application.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Administration.Handlers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserDto>>
{
    private readonly IUserService _userService;
    private readonly ILogger<SearchUsersQueryHandler> _logger;
    
    public SearchUsersQueryHandler(
        IUserService userService,
        ILogger<SearchUsersQueryHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<List<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Admin user initiated search for users. Query: '{Query}', Limit: {Limit}", 
            request.Query, request.Limit);

        if (string.IsNullOrWhiteSpace(request.Query))
        {
            _logger.LogWarning("Search query is null or whitespace");
            return new List<UserDto>();
        }

        var normalizedQuery = request.Query.Trim();

        try
        {
            var results = await _userService.SearchUsersAsync(normalizedQuery, request.Limit, cancellationToken);
            
            _logger.LogDebug("User search completed. Found {Count} users matching query '{Query}'", 
                results.Count, normalizedQuery);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching for users with query '{Query}'", normalizedQuery);
            throw;
        }
    }
}