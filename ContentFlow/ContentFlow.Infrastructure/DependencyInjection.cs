using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Infrastructure.DatabaseEngine;
using ContentFlow.Infrastructure.Identity;
using ContentFlow.Infrastructure.Mappings;
using ContentFlow.Infrastructure.Repositories;
using ContentFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContentFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        
        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        // Repository
        services.AddScoped<IPostRepository, PostRepository>();
        
        // Services
        services.AddScoped<IUserService, UserService>();
        
        // Mappings
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        
        return services;
    }
}