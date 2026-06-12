using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class CommentReaction
{
    #region Properties

    public int Id { get; private set; }
    public int CommentId { get; private set; }
    public int UserId { get; private set; }
    public ReactionType ReactionType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    #endregion
    
    #region Constructors

    public CommentReaction(int commentId, int userId, ReactionType reactionType)
    {
        if (commentId <= 0 || userId <= 0)
        {
            throw new ArgumentException("Arguments commentId or userId are required");
        }
        
        CommentId = commentId;
        UserId = userId;
        ReactionType = reactionType;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion
    
    #region Methods

    public void UpdateReaction(ReactionType reactionType)
    {
        ReactionType = reactionType;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion
}