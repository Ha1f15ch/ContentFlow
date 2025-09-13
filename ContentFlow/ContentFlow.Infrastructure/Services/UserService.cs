using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace ContentFlow.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            Console.WriteLine($"User with email = {email} not found");
            return null;
        }
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<List<UserDto>> GetByIdsAsync(List<int> userIds, CancellationToken ct)
    {
        return await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.AuthorAvatar,
                u.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<UserDto> GetByIdAsync(int userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException($"User with {userId} not found");
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(string email, string password, string? firstName = null, string? lastName = null)
    {
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };
        
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            throw new ValidationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user == null) 
            return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<List<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var users = await _userManager.Users.ToListAsync(ct);
        return _mapper.Map<List<UserDto>>(users);
    }
}