using ContentFlow.Domain.Enums;

namespace ContentFlow.Domain.Entities;

public class Subscription
{
    #region Fields
    public int Id { get; private set; }
    /// <summary>
        /// Кто подписался
        /// </summary>
    public int UserProfileFollowerId { get; private set; } 
    /// <summary>
        /// На кого подписались
        /// </summary>
    public int UserProfileFollowingId { get; private set; }
    /// <summary>
        /// Дата подписки
        /// </summary>
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    /// <summary>
        /// Дата отмены подписки подписчиком
        /// </summary>
    public DateTime? DeactivatedAt { get; private set; }
    /// <summary>
    /// Поставить отображаемый контент на паузу
    /// </summary>
    public bool IsPaused { get; private set; } = false;
    public bool IsActive => !DeactivatedAt.HasValue;
    /// <summary>
    /// Тип подписки
    /// </summary>
    public SubscriptionType SubscriptionType { get; private set; }
    /// <summary>
    /// Уведомлять ли подписчика
    /// </summary>
    public bool NotificationsEnabled { get; private set; }
    
    #endregion
    
    #region Constructors
    public Subscription() {}

    public Subscription(int followerId, int followingId)
    {
        UserProfileFollowerId = followerId;
        UserProfileFollowingId = followingId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeactivatedAt = null;
        SubscriptionType = SubscriptionType.SimpleUser;
        NotificationsEnabled = true;
    }
    #endregion
    
    #region Methods

    /// <summary>
    /// Поставить на-пузу отображение контента.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Pause()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot pause a deactivated subscription.");

        IsPaused = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Снять с паузы. Контент пользователя, на которого подписались, будет отображаться в ленте
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Resume()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot resume a deactivated subscription.");

        IsPaused = false;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Включить уведомления о выходящем контенте пользователя
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void EnableNotifications()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot enable notifications for deactivated subscription.");
        NotificationsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отключить уведомления о выходящем контенте пользователя
    /// </summary>
    public void DisableNotifications()
    {
        NotificationsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Soft-delete подписки. Она удалена, но сохраняется в БД
    /// </summary>
    public void Deactivate()
    {
        if (DeactivatedAt.HasValue) return; // если уже отписан

        DeactivatedAt = DateTime.UtcNow;
        IsPaused = false;
        NotificationsEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Восстановление подписки
    /// </summary>
    public void Reactivate()
    {
        if (!DeactivatedAt.HasValue) return; // уже активна

        DeactivatedAt = null;
        IsPaused = false;
        NotificationsEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion
}