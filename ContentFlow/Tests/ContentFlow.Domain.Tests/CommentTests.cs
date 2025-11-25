using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class CommentTests
{
    private const int PostId = 1;
    private const int AuthorId = 2;

    [Fact]
    public void Constructor_WithValidData_ShouldInitializeComment()
    {
        var comment = new Comment("Great post!", PostId, AuthorId);

        comment.Content.Should().Be("Great post!");
        comment.PostId.Should().Be(PostId);
        comment.AuthorId.Should().Be(AuthorId);
        comment.Status.Should().Be(CommentStatus.Pending);
        comment.IsDeleted.Should().BeFalse();
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        comment.ParentCommentId.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithParentCommentId_ShouldSetParent()
    {
        var comment = new Comment("Reply", PostId, AuthorId, parentCommentId: 100);

        comment.ParentCommentId.Should().Be(100);
    }

    [Fact]
    public void Constructor_WithNullContent_ShouldThrowArgumentException()
    {
        var act = () => new Comment(null!, PostId, AuthorId);
        act.Should().Throw<ArgumentException>().WithMessage("Content is required");
    }

    [Fact]
    public void Constructor_WithEmptyContent_ShouldThrowArgumentException()
    {
        var act = () => new Comment("", PostId, AuthorId);
        act.Should().Throw<ArgumentException>().WithMessage("Content is required");
    }

    [Fact]
    public void Edit_WithValidContent_ShouldUpdateContentAndUpdatedAt()
    {
        var comment = new Comment("Old", PostId, AuthorId);
        var oldUpdatedAt = comment.UpdatedAt;

        comment.Edit("New content");

        comment.Content.Should().Be("New content");
        comment.UpdatedAt.Should().BeAfter(oldUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    public void Edit_WithNullContent_ShouldThrowArgumentException()
    {
        var comment = new Comment("Valid", PostId, AuthorId);
        var act = () => comment.Edit(null!);
        act.Should().Throw<ArgumentException>().WithMessage("Content cannot be empty");
    }

    [Fact]
    public void Delete_ShouldMarkAsDeletedAndSetUpdatedAt()
    {
        var comment = new Comment("Text", PostId, AuthorId);

        comment.Delete();

        comment.IsDeleted.Should().BeTrue();
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldThrowInvalidOperationException()
    {
        var comment = new Comment("Text", PostId, AuthorId);
        comment.Delete();

        var act = () => comment.Delete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Comment is already deleted");
    }

    [Fact]
    public void MarkAsSpam_ShouldSetStatusToSpamAndUpdatedAt()
    {
        var comment = new Comment("Text", PostId, AuthorId);

        comment.MarkAsSpam();

        comment.Status.Should().Be(CommentStatus.Spam);
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Approve_WhenStatusIsPending_ShouldSetStatusToApproved()
    {
        var comment = new Comment("Text", PostId, AuthorId);

        comment.Approve();

        comment.Status.Should().Be(CommentStatus.Approved);
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Approve_WhenStatusIsSpam_ShouldThrowInvalidOperationException()
    {
        var comment = new Comment("Text", PostId, AuthorId);
        comment.MarkAsSpam(); // теперь Spam

        var act = () => comment.Approve();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only pending comments can be approved");
    }
    
    [Fact]
    public void Constructor_WithValidParentCommentId_ShouldSetParentCommentIdCorrectly()
    {
        // Arrange & Act
        var comment = new Comment("Nested comment", PostId, AuthorId, parentCommentId: 42);

        // Assert
        comment.ParentCommentId.Should().Be(42);
    }

    [Fact]
    public void UpdatedAt_ShouldBeNull_ImmediatelyAfterCreation()
    {
        var comment = new Comment("Initial content", PostId, AuthorId);

        comment.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Edit_ShouldSetUpdatedAtToNonNullValue()
    {
        var comment = new Comment("Content", PostId, AuthorId);

        comment.Edit("Updated");

        comment.UpdatedAt.Should().NotBeNull();
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_ShouldSetUpdatedAtToNonNullValue()
    {
        var comment = new Comment("Content", PostId, AuthorId);

        comment.Delete();

        comment.UpdatedAt.Should().NotBeNull();
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsSpam_ShouldSetUpdatedAtToNonNullValue()
    {
        var comment = new Comment("Content", PostId, AuthorId);

        comment.MarkAsSpam();

        comment.UpdatedAt.Should().NotBeNull();
        comment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}