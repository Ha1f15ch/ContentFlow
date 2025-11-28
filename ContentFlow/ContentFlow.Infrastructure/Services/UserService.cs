using AutoMapper;
using ContentFlow.Application.Common;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;

namespace ContentFlow.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager, 
        IMapper mapper, 
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User not found by email: {Email}", email);
            return null;
        }
        
        _logger.LogDebug("User found: {UserId}, Email: {Email}", user.Id, email);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<List<UserDto>> GetByIdsAsync(List<int> userIds, CancellationToken ct)
    {
        _logger.LogDebug("Fetching {UserCount} users by IDs", userIds.Count);
        
        var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserDto(
                u.Id,
                u.Email!,
                u.UserName!,
                u.AuthorAvatar,
                u.CreatedAt,
                u.EmailConfirmed))
            .ToListAsync(ct);
        
        _logger.LogDebug("Fetched {FoundCount}/{RequestedCount} users", users.Count, userIds.Count);
        return users;
    }

    public async Task<UserDto> GetByIdAsync(int userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogError("User not found by ID: {UserId}", userId);
            throw new NotFoundException($"User with ID {userId} was not found.");
        }
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(string email, string password, string userName)
    {
        _logger.LogInformation("Creating new user with email: {Email}", email);

        var user = new ApplicationUser
        {
            Email = email,
            UserName = userName,
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };
        
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var error = $"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            _logger.LogError("User creation failed for {Email}: {Error}", email, error);
            throw new ValidationException(error);
        }

        _logger.LogInformation("User created successfully: {UserId}", user.Id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task AddToRoleAsync(string email, string role, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Cannot add role: user not found. Email: {Email}, Role: {Role}", email, role);
            return;
        }
        
        await _userManager.AddToRoleAsync(user, role);
        _logger.LogInformation("User {Email} added to role: {Role}", email, role);
    }

    public async Task RemoveFromRoleAsync(string email, string role, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Cannot remove role: user not found. Email: {Email}, Role: {Role}", email, role);
            return;
        }
        
        await _userManager.RemoveFromRoleAsync(user, role);
        _logger.LogInformation("User {Email} removed from role: {Role}", email, role);
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogDebug("User {UserId} not found during role check", userId);
            return false;
        }

        var isInRole = await _userManager.IsInRoleAsync(user, role);
        _logger.LogDebug("User {UserId} role '{Role}' check result: {IsInRole}", userId, role, isInRole);
        return isInRole;
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct)
    {
        _logger.LogInformation("Fetching all users");
        var users = await _userManager.Users.ToListAsync(ct);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<bool> CheckPasswordAsync(string email, string password, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogDebug("Password check: user not found. Email: {Email}", email);
            return false;
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);
        _logger.LogDebug("Password check for {Email}: {IsValid}", email, isValid);
        return isValid;
    }

    public async Task<bool> ConfirmEmailAsync(int userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            _logger.LogError("Email confirmation failed: user not found. UserId: {UserId}", userId);
            throw new NotFoundException($"User with ID {userId} was not found.");
        }
        
        if (user.EmailConfirmed)
        {
            _logger.LogDebug("Email already confirmed for user: {UserId}", userId);
            return true;
        }
        
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            var error = $"Failed to confirm email: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            _logger.LogError("Email confirmation failed for user {UserId}: {Error}", userId, error);
            throw new ValidationException(error);
        }

        _logger.LogInformation("Email confirmed for user: {UserId}", userId);
        return true;
    }

    public async Task<List<string>> GetRolesAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("Roles requested for non-existent user: {Email}", email);
            return new List<string>();
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        _logger.LogDebug("Roles for {Email}: [{Roles}]", email, string.Join(", ", roles));
        return [..roles];
    }
    
    public async Task<List<UserDto>> SearchUsersAsync(string query, int limit, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<UserDto>();

        var users = await _userManager.Users
            .Where(u => 
                u.Email!.Contains(query) ||
                (u.FirstName != null && u.FirstName.Contains(query)) ||
                (u.LastName != null && u.LastName.Contains(query)))
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .Select(u => new UserDto(
                u.Id,
                u.Email!,
                u.UserName!,
                u.AuthorAvatar,
                u.CreatedAt,
                u.EmailConfirmed))
            .ToListAsync(ct);

        return users;
    }
    
    public async Task<PaginatedResult<UserDto>> GetBannedUsersAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _userManager.Users.Where(u => u.IsBlocked == true);

        var totalCount = await query.CountAsync(ct);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserDto(
                u.Id,
                u.Email!,
                u.UserName!,
                u.AuthorAvatar,
                u.CreatedAt,
                u.EmailConfirmed))
            .ToListAsync(ct);

        return new PaginatedResult<UserDto>(users, totalCount, page, pageSize);
    }
    
    public async Task BanUserAsync(int userId, string reason, int adminId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException($"User with ID {userId} was not found.");

        if (user.IsBlocked)
        {
            _logger.LogDebug("User {UserId} is already blocked", userId);
            return;
        }

        user.IsBlocked = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new ValidationException($"Failed to block user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }
    
    public async Task UnbanUserAsync(int userId, int adminId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new NotFoundException($"User with ID {userId} was not found.");

        if (!user.IsBlocked)
        {
            _logger.LogDebug("User {UserId} is not blocked, no need to unban", userId);
            return;
        }

        user.IsBlocked = false;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new ValidationException($"Failed to unblock user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
    }

    public async Task<UserDto?> GetUserByUserNameAsync(string userName, CancellationToken ct)
    {
        _logger.LogDebug("Start search user by userName {userName}", userName);
        
        var user  = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            _logger.LogDebug("User {UserName} was not found", userName);
            return null;
        }
        else
        {
            _logger.LogDebug("User {UserName} was found", userName);
            return _mapper.Map<UserDto>(user);
        }
    }

    public async Task<List<UserDto>> GetUsersByUserNameAsync(string partUserName, int limit, CancellationToken ct)
    {
        _logger.LogDebug("Start search user by part userName {partString}", partUserName);

        if (string.IsNullOrWhiteSpace(partUserName))
        {
            return new List<UserDto>();
        }

        var users = await _userManager.Users
            .Where(u => u.UserName!.Contains(partUserName))
            .OrderBy(u => u.UserName)
            .Take(limit)
            .Select(u => new UserDto(
                u.Id,
                u.Email!,
                u.UserName!,
                u.AuthorAvatar,
                u.CreatedAt,
                u.EmailConfirmed))
            .ToListAsync(ct);

        _logger.LogDebug("Finish search user by part userName {partString}. Founded {countUserRecords}", partUserName,  users.Count);
        
        return users;
    }
}