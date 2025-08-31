namespace ContentFlow.Domain.Enums;

public enum PostStatus
{
    Draft, // Черновик — только автор видит
    PendingModeration, // На модерации
    Published, // Опубликован
    Rejected, // Отклонён модератором
    Archived // Снят с публикации
}