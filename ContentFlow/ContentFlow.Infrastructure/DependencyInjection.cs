using ContentFlow.Application.Interfaces.Category;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.Common.Jobs;
using ContentFlow.Application.Interfaces.FileStorage;
using ContentFlow.Application.Interfaces.Notification;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.RefreshToken;
using ContentFlow.Application.Interfaces.Subscription;
using ContentFlow.Application.Interfaces.Tag;
using ContentFlow.Application.Interfaces.UserProfile;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Infrastructure.Configuration;
using ContentFlow.Infrastructure.DatabaseEngine;
using ContentFlow.Infrastructure.Identity;
using ContentFlow.Infrastructure.Jobs;
using ContentFlow.Infrastructure.Mappings;
using ContentFlow.Infrastructure.Notifications.SignalR;
using ContentFlow.Infrastructure.Repositories;
using ContentFlow.Infrastructure.Services;
using Hangfire;
using Hangfire.Console;
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
        
        // Hangfire
        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString)
                .UseSerilogLogProvider() // serilog в hangfire
                .UseConsole();
        });
        
        services.AddHangfireServer();
        
        // SignalR
        services.AddScoped<NotificationsHub>();
        
        // Jobs
        services.AddScoped<TokenCleanupJob>();
        services.AddScoped<INotificationSenderJob, NotificationSenderJob>();
        
        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        // Repository
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUserTwoFactorCodeRepository, UserTwoFactorCodeRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
        services.AddTransient<INotificationRepository, NotificationRepository>();
        
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPostCommentsService, PostCommentsService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRealtimeNotificationSender, SignalRNotificationSender>();
        
        // Mappings
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        
        //Configurations
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        return services;
    }
}