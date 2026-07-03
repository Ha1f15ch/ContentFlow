using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class ModerationCase
{
    private const int MaxModeratorNoteLength = 2000;

    #region Properties

    public int Id { get; private set; }
    public int? PostId { get; private set; }
    public int? CommentId { get; private set; }
    public ModerationCaseStatus Status { get; private set; }
    public ModerationPriority Priority { get; private set; }
    public int ReportCount { get; private set; }
    public int UniqueReporterCount { get; private set; }
    public DateTime FirstReportedAt { get; private set; }
    public DateTime LastReportedAt { get; private set; }
    public int? AssignedModeratorId { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public int? ResolvedById { get; private set; }
    public ModerationDecision? Decision { get; private set; }
    public string? ModeratorNote { get; private set; }
    public DateTime CreatedAt { get; private set; }

    #endregion

    #region Constructor

    protected ModerationCase()
    {
    }

    private ModerationCase(int? postId, int? commentId)
    {
        ValidateTarget(postId, commentId);

        PostId = postId;
        CommentId = commentId;
        Status = ModerationCaseStatus.Open;
        Priority = ModerationPriority.Low;
        ReportCount = 0;
        UniqueReporterCount = 0;
        CreatedAt = DateTime.UtcNow;
        FirstReportedAt = CreatedAt;
        LastReportedAt = CreatedAt;
    }

    #endregion

    #region Factory Methods

    public static ModerationCase OpenForPost(int postId)
    {
        if (postId <= 0)
            throw new ArgumentOutOfRangeException(nameof(postId));

        return new ModerationCase(postId, null);
    }

    public static ModerationCase OpenForComment(int commentId)
    {
        if (commentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(commentId));

        return new ModerationCase(null, commentId);
    }

    #endregion

    #region Public Methods

    public bool IsOpen =>
        Status is ModerationCaseStatus.Open or ModerationCaseStatus.InReview;

    public bool IsForPost => PostId.HasValue;

    public bool IsForComment => CommentId.HasValue;

    public bool ShouldAutoHideContent =>
        Priority is ModerationPriority.High or ModerationPriority.Critical
        || UniqueReporterCount >= 5
        || ReportCount >= 5;

    public void RegisterReport(ReportReasonType reasonType, bool isUniqueReporter)
    {
        if (!IsOpen)
            throw new InvalidOperationException("Cannot register a report for a closed moderation case.");

        ReportCount++;
        if (isUniqueReporter)
            UniqueReporterCount++;

        LastReportedAt = DateTime.UtcNow;
        RecalculatePriority(reasonType);
    }

    public void TakeInReview(int moderatorId)
    {
        if (moderatorId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moderatorId));

        if (!IsOpen)
            throw new InvalidOperationException("Only open moderation cases can be taken in review.");

        Status = ModerationCaseStatus.InReview;
        AssignedModeratorId = moderatorId;
    }

    public void ReleaseFromReview()
    {
        if (Status != ModerationCaseStatus.InReview)
            throw new InvalidOperationException("Only cases in review can be released.");

        Status = ModerationCaseStatus.Open;
        AssignedModeratorId = null;
    }

    public void Resolve(ModerationDecision decision, int moderatorId, string? note = null)
    {
        if (moderatorId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moderatorId));

        if (!IsOpen)
            throw new InvalidOperationException("Only open moderation cases can be resolved.");

        ValidateModeratorNote(note);

        Decision = decision;
        ModeratorNote = NormalizeModeratorNote(note);
        ResolvedById = moderatorId;
        ResolvedAt = DateTime.UtcNow;
        Status = ModerationCaseStatus.Resolved;
        AssignedModeratorId = moderatorId;
    }

    public void Dismiss(int moderatorId, string? note = null)
    {
        if (moderatorId <= 0)
            throw new ArgumentOutOfRangeException(nameof(moderatorId));

        if (!IsOpen)
            throw new InvalidOperationException("Only open moderation cases can be dismissed.");

        ValidateModeratorNote(note);

        Decision = ModerationDecision.NoAction;
        ModeratorNote = NormalizeModeratorNote(note);
        ResolvedById = moderatorId;
        ResolvedAt = DateTime.UtcNow;
        Status = ModerationCaseStatus.Dismissed;
        AssignedModeratorId = moderatorId;
    }

    public bool RequiresContentHidden(ModerationDecision decision) =>
        decision is ModerationDecision.ContentHidden or ModerationDecision.ContentRemoved;

    public bool RequiresAuthorAction(ModerationDecision decision) =>
        decision is ModerationDecision.AuthorWarned
            or ModerationDecision.AuthorTempBanned
            or ModerationDecision.AuthorPermBanned;

    #endregion

    #region Private Methods

    private static void ValidateTarget(int? postId, int? commentId)
    {
        var hasPost = postId.HasValue;
        var hasComment = commentId.HasValue;

        if (hasPost == hasComment)
            throw new ArgumentException("Moderation case must target either a post or a comment, but not both.");
    }

    private static void ValidateModeratorNote(string? note)
    {
        if (note != null && note.Length > MaxModeratorNoteLength)
            throw new ArgumentException($"Moderator note cannot exceed {MaxModeratorNoteLength} characters.", nameof(note));
    }

    private static string? NormalizeModeratorNote(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
            return null;

        return note.Trim();
    }

    private void RecalculatePriority(ReportReasonType latestReason)
    {
        var reasonPriority = latestReason switch
        {
            ReportReasonType.HateSpeech => ModerationPriority.High,
            ReportReasonType.Harassment => ModerationPriority.High,
            ReportReasonType.Nsfw => ModerationPriority.Medium,
            ReportReasonType.Misinformation => ModerationPriority.Medium,
            ReportReasonType.Spam => ModerationPriority.Medium,
            _ => ModerationPriority.Low
        };

        var volumePriority = UniqueReporterCount switch
        {
            >= 10 => ModerationPriority.Critical,
            >= 5 => ModerationPriority.High,
            >= 3 => ModerationPriority.Medium,
            _ => ModerationPriority.Low
        };

        Priority = (ModerationPriority)Math.Max((int)reasonPriority, (int)volumePriority);

        if (ReportCount >= 10)
            Priority = ModerationPriority.Critical;
    }

    #endregion
}
