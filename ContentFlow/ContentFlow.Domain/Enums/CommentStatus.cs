namespace ContentFlow.Domain.Enums;

public enum CommentStatus
{
    Pending,
    Approved,
    Rejected,
    Spam,
    HiddenPendingReview // Скрыт до решения модератора по жалобе
}