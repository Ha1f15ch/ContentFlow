using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class Comment
{
    #region Class Comment properties

    public int Id { get; private set; }
    public string Content { get; private set; } = String.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    public int PostId { get; private set; }
    public int AuthorId { get; private set; }
    public int? ParentCommentId { get; private set; } // Вложенность комментариев

    public CommentStatus Status { get; private set; } = CommentStatus.Approved;
    public bool IsDeleted { get; private set; } = false;

    #endregion
    
    #region Costructor

    public Comment(string content, int postId, int authorId, int? parentCommentId = null)
    {
        if(string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required");
        
        Content = content;
        PostId = postId;
        AuthorId = authorId;
        ParentCommentId = parentCommentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = CommentStatus.Approved;
    }

    #endregion
    
    #region Public Methods
    public void Edit(string newContent)
    {
        if(string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Content cannot be empty");
        
        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Comment is already deleted");

        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsSpam()
    {
        Status = CommentStatus.Spam;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status == CommentStatus.Approved)
        {
            return;
        }
        else if (Status != CommentStatus.Pending)
        {
            throw new InvalidOperationException("Only pending comments can be approved");
        }
        
        Status = CommentStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void HideForModerationReview()
    {
        if (Status == CommentStatus.HiddenPendingReview)
            return;

        if (Status != CommentStatus.Approved && Status != CommentStatus.Pending)
            throw new InvalidOperationException("Only approved or pending comments can be hidden for moderation review.");

        Status = CommentStatus.HiddenPendingReview;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RestoreAfterModeration()
    {
        if (Status != CommentStatus.HiddenPendingReview)
            throw new InvalidOperationException("Only comments hidden for moderation review can be restored.");

        Status = CommentStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveByModerator()
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        Status = CommentStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion
}