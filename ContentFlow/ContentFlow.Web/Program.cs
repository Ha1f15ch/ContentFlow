using ContentFlow.Application;
using ContentFlow.Application.Common;
using ContentFlow.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // for react
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication();

// controllers
builder.Services.AddControllers();

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

var app = builder.Build();

await AutoInitializeRole(app.Services);

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