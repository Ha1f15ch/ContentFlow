namespace ContentFlow.Domain.Enums;

public enum NotificationType
{
    NewPost = 1,
    NewSubscriber = 2,
    CommentOnPost = 3,
    ReplyToComment = 4,
    Mention = 5,
    System = 100
}