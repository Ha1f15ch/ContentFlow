using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class Report
{
    private const int MaxDescriptionLength = 2000;

    #region Properties

    public int Id { get; private set; }
    public int ReporterId { get; private set; }
    public int? PostId { get; private set; }
    public int? CommentId { get; private set; }
    public ReportReasonType ReasonType { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    #endregion

    #region Constructor

    protected Report()
    {
    }

    public Report(
        int reporterId,
        ReportReasonType reasonType,
        int? postId = null,
        int? commentId = null,
        string? description = null)
    {
        if (reporterId <= 0)
            throw new ArgumentOutOfRangeException(nameof(reporterId));

        ValidateTarget(postId, commentId);
        ValidateDescription(description);

        ReporterId = reporterId;
        ReasonType = reasonType;
        PostId = postId;
        CommentId = commentId;
        Description = NormalizeDescription(description);
        CreatedAt = DateTime.UtcNow;
    }

    #endregion

    #region Public Methods

    public bool IsForPost => PostId.HasValue;

    public bool IsForComment => CommentId.HasValue;

    public static Report ForPost(
        int reporterId,
        int postId,
        ReportReasonType reasonType,
        string? description = null)
    {
        return new Report(reporterId, reasonType, postId: postId, description: description);
    }

    public static Report ForComment(
        int reporterId,
        int commentId,
        ReportReasonType reasonType,
        string? description = null)
    {
        return new Report(reporterId, reasonType, commentId: commentId, description: description);
    }

    public void UpdateReason(ReportReasonType reasonType, string? description = null)
    {
        ValidateDescription(description);
        ReasonType = reasonType;
        Description = NormalizeDescription(description);
    }

    #endregion

    #region Private Methods

    private static void ValidateTarget(int? postId, int? commentId)
    {
        var hasPost = postId.HasValue;
        var hasComment = commentId.HasValue;

        if (hasPost == hasComment)
            throw new ArgumentException("Report must target either a post or a comment, but not both.");

        if (hasPost && postId <= 0)
            throw new ArgumentOutOfRangeException(nameof(postId));

        if (hasComment && commentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(commentId));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > MaxDescriptionLength)
            throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters.", nameof(description));
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        return description.Trim();
    }

    #endregion
}
