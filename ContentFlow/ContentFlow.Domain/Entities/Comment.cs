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
        Status = CommentStatus.Pending;
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
        if(Status != CommentStatus.Pending)
            throw new InvalidOperationException("Only pending comments can be approved");
        
        Status = CommentStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion
}