using ContentFlow.Application;
using ContentFlow.Application.Common;
using ContentFlow.Infrastructure;
using ContentFlow.Infrastructure.Jobs;
using ContentFlow.Infrastructure.Notifications.SignalR;
using ContentFlow.Web.Extensions;
using ContentFlow.Web.Security;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Color for logging
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.IncludeScopes = true;
    options.TimestampFormat = "hh:mm:ss ";
});

// Configuration serilog
var logPath = builder.Configuration["Logging:LogPath"] ?? "logs";

try
{
    Directory.CreateDirectory(logPath);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Уменьшаем шум от фреймворка
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .Enrich.FromLogContext() // Добавляет контекст 
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "Logs/log.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileTimeLimit: TimeSpan.FromDays(90),
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 1_000_000, // 1 MB
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}",
            encoding: System.Text.Encoding.UTF8)
        .CreateLogger();

    builder.Host.UseSerilog(); // Заменяет встроенный логгер
}
catch (Exception ex)
{
    Console.WriteLine($"Serilog initialization failed: {ex.Message}");
    throw;
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173") // for react
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication();

// controllers
builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ContentFlow", Version = "v1" });

    // support JWT in swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Add Application (CQRS, MediatR, Validators)
builder.Services.AddApplication();

// Add Infrastructure (DbContext, Identity, Repositories, Email, Mappings)
builder.Services.AddInfrastructure(builder.Configuration);

//Policy for endpoints
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("CanPublish", policy => 
        policy.RequireRole(
            RoleConstants.User,
            RoleConstants.Moderator,
            RoleConstants.Admin));
    
    option.AddPolicy("CanEditContent",  policy =>
        policy.RequireRole(
            RoleConstants.User,
            RoleConstants.ContentEditor,
            RoleConstants.Moderator,
            RoleConstants.Admin));
    
    option.AddPolicy("CanDeleteContent",  policy =>
        policy.RequireRole(
            RoleConstants.User,
            RoleConstants.Moderator,
            RoleConstants.Admin));
    
    option.AddPolicy("AdministrationDictionary",  policy =>
        policy.RequireRole(
            RoleConstants.ContentEditor,
            RoleConstants.Moderator,
            RoleConstants.Admin));
});

var app = builder.Build();

// Initialize database, apply migrations
await app.InitializeDatabaseAsync();

// Initialize role
await AutoInitializeRole(app.Services);

// Turn on hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    //Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentFlow API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowLocalFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR настройка хаба
app.MapHub<NotificationsHub>("/hubs/notifications");

// Start work job scheduler
using (var scope = app.Services.CreateScope())
{
    BackgroundJobScheduler.ScheduleJobs(scope.ServiceProvider.GetRequiredService<IRecurringJobManager>());
}

app.Run();

static async Task AutoInitializeRole(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    var roleNames = new[]
    {
        RoleConstants.Guest.ToString(),
        RoleConstants.User.ToString(),
        RoleConstants.ContentEditor.ToString(),
        RoleConstants.Moderator.ToString(),
        RoleConstants.Admin.ToString(),
        RoleConstants.Banned.ToString()
    };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            Console.WriteLine($"Role {roleName} not founded, create the new.");
        }   
    }
}