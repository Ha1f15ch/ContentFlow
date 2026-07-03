using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class ModerationCaseTests
{
    private const int PostId = 10;
    private const int CommentId = 20;
    private const int ModeratorId = 5;

    [Fact]
    public void OpenForPost_ShouldInitializeOpenCase()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.PostId.Should().Be(PostId);
        moderationCase.CommentId.Should().BeNull();
        moderationCase.Status.Should().Be(ModerationCaseStatus.Open);
        moderationCase.Priority.Should().Be(ModerationPriority.Low);
        moderationCase.ReportCount.Should().Be(0);
        moderationCase.UniqueReporterCount.Should().Be(0);
        moderationCase.IsOpen.Should().BeTrue();
        moderationCase.IsForPost.Should().BeTrue();
        moderationCase.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void OpenForComment_ShouldInitializeOpenCase()
    {
        var moderationCase = ModerationCase.OpenForComment(CommentId);

        moderationCase.CommentId.Should().Be(CommentId);
        moderationCase.PostId.Should().BeNull();
        moderationCase.IsForComment.Should().BeTrue();
    }

    [Fact]
    public void RegisterReport_WithUniqueReporter_ShouldIncrementCountersAndPriority()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.RegisterReport(ReportReasonType.HateSpeech, isUniqueReporter: true);

        moderationCase.ReportCount.Should().Be(1);
        moderationCase.UniqueReporterCount.Should().Be(1);
        moderationCase.Priority.Should().Be(ModerationPriority.High);
        moderationCase.LastReportedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RegisterReport_WithDuplicateReporter_ShouldIncrementOnlyReportCount()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);
        moderationCase.RegisterReport(ReportReasonType.Other, isUniqueReporter: true);

        moderationCase.RegisterReport(ReportReasonType.Other, isUniqueReporter: false);

        moderationCase.ReportCount.Should().Be(2);
        moderationCase.UniqueReporterCount.Should().Be(1);
    }

    [Fact]
    public void RegisterReport_WhenManyUniqueReporters_ShouldSetCriticalPriority()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        for (var i = 0; i < 10; i++)
            moderationCase.RegisterReport(ReportReasonType.Other, isUniqueReporter: true);

        moderationCase.ReportCount.Should().Be(10);
        moderationCase.UniqueReporterCount.Should().Be(10);
        moderationCase.Priority.Should().Be(ModerationPriority.Critical);
        moderationCase.ShouldAutoHideContent.Should().BeTrue();
    }

    [Fact]
    public void RegisterReport_WhenCaseClosed_ShouldThrowInvalidOperationException()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);
        moderationCase.Dismiss(ModeratorId);

        var act = () => moderationCase.RegisterReport(ReportReasonType.Spam, isUniqueReporter: true);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot register a report for a closed moderation case.");
    }

    [Fact]
    public void TakeInReview_ShouldAssignModeratorAndSetStatus()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.TakeInReview(ModeratorId);

        moderationCase.Status.Should().Be(ModerationCaseStatus.InReview);
        moderationCase.AssignedModeratorId.Should().Be(ModeratorId);
        moderationCase.IsOpen.Should().BeTrue();
    }

    [Fact]
    public void ReleaseFromReview_ShouldResetModeratorAndStatus()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);
        moderationCase.TakeInReview(ModeratorId);

        moderationCase.ReleaseFromReview();

        moderationCase.Status.Should().Be(ModerationCaseStatus.Open);
        moderationCase.AssignedModeratorId.Should().BeNull();
    }

    [Fact]
    public void Resolve_ShouldSetDecisionAndCloseCase()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);
        moderationCase.RegisterReport(ReportReasonType.Spam, isUniqueReporter: true);

        moderationCase.Resolve(ModerationDecision.ContentRemoved, ModeratorId, "Confirmed spam");

        moderationCase.Status.Should().Be(ModerationCaseStatus.Resolved);
        moderationCase.Decision.Should().Be(ModerationDecision.ContentRemoved);
        moderationCase.ResolvedById.Should().Be(ModeratorId);
        moderationCase.ModeratorNote.Should().Be("Confirmed spam");
        moderationCase.ResolvedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        moderationCase.IsOpen.Should().BeFalse();
    }

    [Fact]
    public void Dismiss_ShouldSetNoActionDecisionAndCloseCase()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.Dismiss(ModeratorId, "False report");

        moderationCase.Status.Should().Be(ModerationCaseStatus.Dismissed);
        moderationCase.Decision.Should().Be(ModerationDecision.NoAction);
        moderationCase.ModeratorNote.Should().Be("False report");
        moderationCase.IsOpen.Should().BeFalse();
    }

    [Fact]
    public void RequiresContentHidden_ShouldReturnTrueForHiddenOrRemovedDecisions()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.RequiresContentHidden(ModerationDecision.ContentHidden).Should().BeTrue();
        moderationCase.RequiresContentHidden(ModerationDecision.ContentRemoved).Should().BeTrue();
        moderationCase.RequiresContentHidden(ModerationDecision.NoAction).Should().BeFalse();
    }

    [Fact]
    public void RequiresAuthorAction_ShouldReturnTrueForAuthorDecisions()
    {
        var moderationCase = ModerationCase.OpenForPost(PostId);

        moderationCase.RequiresAuthorAction(ModerationDecision.AuthorWarned).Should().BeTrue();
        moderationCase.RequiresAuthorAction(ModerationDecision.AuthorTempBanned).Should().BeTrue();
        moderationCase.RequiresAuthorAction(ModerationDecision.AuthorPermBanned).Should().BeTrue();
        moderationCase.RequiresAuthorAction(ModerationDecision.ContentHidden).Should().BeFalse();
    }

    [Fact]
    public void OpenForPost_WithInvalidId_ShouldThrowArgumentOutOfRangeException()
    {
        var act = () => ModerationCase.OpenForPost(0);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("postId");
    }
}
