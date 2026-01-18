namespace ContentFlow.Domain.Enums;

public enum NotificationType
{
    NewPost = 1,
    CommentOnPost = 2,
    ReplyToComment = 3,
    Mention = 4,
    System = 100
}