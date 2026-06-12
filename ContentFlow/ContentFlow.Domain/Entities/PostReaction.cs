using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class PostReaction
{
    #region Properties
    public int Id { get; private set; }
    public int PostId { get; private set; }
    public int UserId { get; private set; }
    public ReactionType ReactionType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    #endregion
    
    #region Constructor of PostReaction

    public PostReaction(int postId, int userId, ReactionType reactionType)
    {
        if (postId <= 0 || userId <= 0)
        {
            throw new ArgumentException("Arguments postId or userId are required");
        }
        
        PostId = postId;
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