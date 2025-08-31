using AutoMapper;
using ContentFlow.Application.DTOs;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Exceptions;
using ContentFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

    public async Task<UserDto> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException($"uer with {email} not found");
        
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetByIdAsync(string userId, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User with {userId} not found");
        
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