using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class ReportTests
{
    private const int ReporterId = 1;
    private const int PostId = 10;
    private const int CommentId = 20;

    [Fact]
    public void ForPost_WithValidData_ShouldInitializeReport()
    {
        var report = Report.ForPost(ReporterId, PostId, ReportReasonType.Spam, "Looks like spam");

        report.ReporterId.Should().Be(ReporterId);
        report.PostId.Should().Be(PostId);
        report.CommentId.Should().BeNull();
        report.ReasonType.Should().Be(ReportReasonType.Spam);
        report.Description.Should().Be("Looks like spam");
        report.IsForPost.Should().BeTrue();
        report.IsForComment.Should().BeFalse();
        report.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ForComment_WithValidData_ShouldInitializeReport()
    {
        var report = Report.ForComment(ReporterId, CommentId, ReportReasonType.Harassment);

        report.ReporterId.Should().Be(ReporterId);
        report.CommentId.Should().Be(CommentId);
        report.PostId.Should().BeNull();
        report.ReasonType.Should().Be(ReportReasonType.Harassment);
        report.Description.Should().BeNull();
        report.IsForComment.Should().BeTrue();
        report.IsForPost.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithBothTargets_ShouldThrowArgumentException()
    {
        var act = () => new Report(ReporterId, ReportReasonType.Other, postId: PostId, commentId: CommentId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Report must target either a post or a comment, but not both.");
    }

    [Fact]
    public void Constructor_WithNoTarget_ShouldThrowArgumentException()
    {
        var act = () => new Report(ReporterId, ReportReasonType.Other);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Report must target either a post or a comment, but not both.");
    }

    [Fact]
    public void Constructor_WithInvalidReporterId_ShouldThrowArgumentOutOfRangeException()
    {
        var act = () => Report.ForPost(0, PostId, ReportReasonType.Spam);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("reporterId");
    }

    [Fact]
    public void Constructor_WithInvalidPostId_ShouldThrowArgumentOutOfRangeException()
    {
        var act = () => Report.ForPost(ReporterId, 0, ReportReasonType.Spam);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("postId");
    }

    [Fact]
    public void Constructor_WithInvalidCommentId_ShouldThrowArgumentOutOfRangeException()
    {
        var act = () => Report.ForComment(ReporterId, 0, ReportReasonType.Spam);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("commentId");
    }

    [Fact]
    public void Constructor_WithTooLongDescription_ShouldThrowArgumentException()
    {
        var longDescription = new string('a', 2001);

        var act = () => Report.ForPost(ReporterId, PostId, ReportReasonType.Other, longDescription);

        act.Should().Throw<ArgumentException>().WithParameterName("description");
    }

    [Fact]
    public void Constructor_WithWhitespaceDescription_ShouldNormalizeToNull()
    {
        var report = Report.ForPost(ReporterId, PostId, ReportReasonType.Other, "   ");

        report.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateReason_ShouldUpdateReasonTypeAndDescription()
    {
        var report = Report.ForPost(ReporterId, PostId, ReportReasonType.Spam);

        report.UpdateReason(ReportReasonType.HateSpeech, "Offensive language");

        report.ReasonType.Should().Be(ReportReasonType.HateSpeech);
        report.Description.Should().Be("Offensive language");
    }
}
