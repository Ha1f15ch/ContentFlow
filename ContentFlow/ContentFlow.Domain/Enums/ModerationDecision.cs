namespace ContentFlow.Domain.Enums;

public enum ModerationDecision
{
    NoAction,
    ContentHidden,
    ContentRemoved,
    AuthorWarned,
    AuthorTempBanned,
    AuthorPermBanned
}
