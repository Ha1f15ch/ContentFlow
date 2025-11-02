using Hangfire;

namespace ContentFlow.Infrastructure.Jobs;

public class BackgroundJobScheduler
{
    public static void ScheduleJobs(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate<TokenCleanupJob>(
            "cleanup-expired-refresh-tokens",
            job => job.CleanupExpiredTokensAsync(),
            Cron.Daily);
    }
}