using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class Notification
{
    #region Property

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Payload { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    #endregion
    
    #region Constructor
    
    protected Notification()
    {
    }
    
    public Notification(int userId, NotificationType type, string payload)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }
        
        UserId = userId;
        Type = type;
        Payload = payload;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    #endregion
    
    #region Methods
    
    public void MarkRead()
    {
        if(!IsRead)
            IsRead = true;
    }
    
    #endregion
}