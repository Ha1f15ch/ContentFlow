using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using FluentAssertions;

namespace ContentFlow.Domain.Tests;

public class PostTests
{
    private const int AuthorId = 1;
    
    [Fact]
    public void Constructor_WithValidData_ShouldInitializePost()
    {
        var post = new Post("My Title", "My Content", AuthorId);

        post.Title.Should().Be("My Title");
        post.Content.Should().Be("My Content");
        post.Status.Should().Be(PostStatus.Draft);
        post.AuthorId.Should().Be(AuthorId);
        post.IsDeleted.Should().BeFalse();
        post.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public void Constructor_WithNullTitle_ShouldThrowArgumentException()
    {
        var act = () => new Post(null!, "Content", AuthorId);
        act.Should().Throw<ArgumentException>().WithParameterName("title");
    }

    [Fact]
    public void Constructor_WithEmptyContent_ShouldThrowArgumentException()
    {
        var act = () => new Post("Title", "", AuthorId);
        act.Should().Throw<ArgumentException>().WithParameterName("content");
    }

    [Fact]
    public void Publish_WhenStatusIsDraft_ShouldSetStatusToPublished()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();

        post.Status.Should().Be(PostStatus.Published);
        post.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Publish_WhenStatusIsPublished_ShouldThrowInvalidOperationException()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish(); // now published

        var act = () => post.Publish();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddTag_WithNewTag_ShouldAddPostTag()
    {
        var post = new Post("Title", "Content", AuthorId);
        var tag = new Tag(1, "C#");

        post.AddTag(tag);

        post.PostTags.Should().ContainSingle(pt => pt.TagId == 1);
    }

    [Fact]
    public void AddTag_WithDuplicateTag_ShouldNotAddDuplicate()
    {
        var post = new Post("Title", "Content", AuthorId);
        var tag = new Tag (1, "C#");

        post.AddTag(tag);
        post.AddTag(tag);

        post.PostTags.Should().ContainSingle();
    }

    [Fact]
    public void GenerateSlug_ShouldConvertTitleToUrlFriendly()
    {
        var post = new Post("Hello World! How are you?", "Content", AuthorId);
        post.Slug.Should().Be("hello-world-how-are-you");
    }

    [Fact]
    public void Truncate_ShouldLimitExcerptTo200Chars()
    {
        var longContent = new string('a', 300);
        var post = new Post("Title", longContent, AuthorId, excerpt: null);

        post.Excerpt.Should().StartWith(new string('a', 200)).And.EndWith("...");
    }

    [Fact]
    public void UpdateContent_ShouldUpdateExcerptAndUpdatedAt()
    {
        var post = new Post("Title", "Old content", AuthorId);
        var oldUpdatedAt = post.UpdatedAt;

        post.UpdateContent("New content");

        post.Content.Should().Be("New content");
        post.Excerpt.Should().Be("New content");
        post.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Fact]
    public void MarkAsDeleted_ShouldSetIsDeletedAndDeletedAt()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.MarkAsDeleted();

        post.IsDeleted.Should().BeTrue();
        post.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void HideForModerationReview_WhenPublished_ShouldSetHiddenPendingReview()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();

        post.HideForModerationReview();

        post.Status.Should().Be(PostStatus.HiddenPendingReview);
        post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void HideForModerationReview_WhenAlreadyHidden_ShouldNotChangeStatus()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();
        post.HideForModerationReview();
        var updatedAt = post.UpdatedAt;

        post.HideForModerationReview();

        post.Status.Should().Be(PostStatus.HiddenPendingReview);
        post.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void HideForModerationReview_WhenDraft_ShouldThrowInvalidOperationException()
    {
        var post = new Post("Title", "Content", AuthorId);

        var act = () => post.HideForModerationReview();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only published or pending posts can be hidden for moderation review.");
    }

    [Fact]
    public void RestoreAfterModeration_WhenHiddenPendingReview_ShouldSetPublished()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();
        post.HideForModerationReview();

        post.RestoreAfterModeration();

        post.Status.Should().Be(PostStatus.Published);
        post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RestoreAfterModeration_WhenNotHidden_ShouldThrowInvalidOperationException()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();

        var act = () => post.RestoreAfterModeration();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only posts hidden for moderation review can be restored.");
    }

    [Fact]
    public void RemoveByModerator_ShouldSetStatusToRejected()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();

        post.RemoveByModerator();

        post.Status.Should().Be(PostStatus.Rejected);
        post.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RemoveByModerator_WhenAlreadyRejected_ShouldNotThrow()
    {
        var post = new Post("Title", "Content", AuthorId);
        post.Publish();
        post.RemoveByModerator();

        var act = () => post.RemoveByModerator();

        act.Should().NotThrow();
        post.Status.Should().Be(PostStatus.Rejected);
    }
}