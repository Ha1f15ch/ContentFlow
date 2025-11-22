using ContentFlow.Infrastructure.DatabaseEngine;
using Microsoft.EntityFrameworkCore;

namespace ContentFlow.Web.Extensions;

public static class HostExtensions
{
    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        // Проверяем, не запущено ли это в CI
        var isRunningInGitHubActions = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        
        if (isRunningInGitHubActions)
        {
            Console.WriteLine("Running in GitHub Actions — skipping database initialization.");
            return;
        }

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("База данных инициализирована");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ошибка при инициализации базы данных");
            throw;
        }
    }
}