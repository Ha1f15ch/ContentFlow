using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Moderation;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Infrastructure.Services;

public class ModerationService : IModerationService
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserService _userService;

    public ModerationService(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserService userService)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userService = userService;
    }

    public async Task EnsureModeratorAsync(int userId, CancellationToken ct)
    {
        var isModerator = await _userService.IsInRoleAsync(userId, RoleConstants.Moderator) ||
                          await _userService.IsInRoleAsync(userId, RoleConstants.Admin);

        if (!isModerator)
            throw new UnauthorizedAccessException("You do not have permission to perform moderation actions.");
    }

    public async Task ApplyDecisionAsync(
        ModerationCase moderationCase,
        ModerationDecision decision,
        int moderatorId,
        string? note,
        CancellationToken ct)
    {
        if (moderationCase.IsForPost)
        {
            await ApplyForPostAsync(moderationCase.PostId!.Value, decision, moderatorId, note, ct);
            return;
        }

        await ApplyForCommentAsync(moderationCase.CommentId!.Value, decision, moderatorId, note, ct);
    }

    public async Task RestoreContentIfHiddenAsync(ModerationCase moderationCase, CancellationToken ct)
    {
        if (moderationCase.IsForPost)
        {
            var post = await _postRepository.GetByIdAsync(moderationCase.PostId!.Value, ct);
            if (post == null)
                throw new NotFoundException("Post not found");

            if (post.Status == PostStatus.HiddenPendingReview)
                post.RestoreAfterModeration();

            await _postRepository.UpdateAsync(post, ct);
            return;
        }

        var comment = await _commentRepository.GetByIdAsync(moderationCase.CommentId!.Value, ct);
        if (comment == null)
            throw new NotFoundException("Comment not found");

        if (comment.Status == CommentStatus.HiddenPendingReview)
            comment.RestoreAfterModeration();

        await _commentRepository.UpdateAsync(comment, ct);
    }

    private async Task ApplyForPostAsync(
        int postId,
        ModerationDecision decision,
        int moderatorId,
        string? note,
        CancellationToken ct)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post == null)
            throw new NotFoundException("Post not found");

        switch (decision)
        {
            case ModerationDecision.ContentHidden:
                post.HideForModerationReview();
                break;
            case ModerationDecision.ContentRemoved:
                post.RemoveByModerator();
                break;
            case ModerationDecision.NoAction:
                if (post.Status == PostStatus.HiddenPendingReview)
                    post.RestoreAfterModeration();
                break;
        }

        await _postRepository.UpdateAsync(post, ct);
        await ApplyAuthorDecisionAsync(post.AuthorId, decision, moderatorId, note, ct);
    }

    private async Task ApplyForCommentAsync(
        int commentId,
        ModerationDecision decision,
        int moderatorId,
        string? note,
        CancellationToken ct)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId, ct);
        if (comment == null)
            throw new NotFoundException("Comment not found");

        switch (decision)
        {
            case ModerationDecision.ContentHidden:
                comment.HideForModerationReview();
                break;
            case ModerationDecision.ContentRemoved:
                comment.RemoveByModerator();
                break;
            case ModerationDecision.NoAction:
                if (comment.Status == CommentStatus.HiddenPendingReview)
                    comment.RestoreAfterModeration();
                break;
        }

        await _commentRepository.UpdateAsync(comment, ct);
        await ApplyAuthorDecisionAsync(comment.AuthorId, decision, moderatorId, note, ct);
    }

    private async Task ApplyAuthorDecisionAsync(
        int authorId,
        ModerationDecision decision,
        int moderatorId,
        string? note,
        CancellationToken ct)
    {
        if (decision is not (ModerationDecision.AuthorTempBanned or ModerationDecision.AuthorPermBanned))
            return;

        var reason = string.IsNullOrWhiteSpace(note)
            ? $"Moderation decision: {decision}"
            : note;

        await _userService.BanUserAsync(authorId, reason, moderatorId, ct);
    }
}
