namespace ContentFlow.Application.Interfaces.Common.Jobs;

public interface INotificationSenderJob
{
    Task SendBatchAsync(List<int> userIds, int postId, int authorId, CancellationToken ct);
}