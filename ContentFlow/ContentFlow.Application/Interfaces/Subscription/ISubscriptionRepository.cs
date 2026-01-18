using ContentFlow.Application.DTOs.UserProfileSubscriptionDTOs;
using SubscriptionDomainModel = ContentFlow.Domain.Entities.Subscription;

namespace ContentFlow.Application.Interfaces.Subscription;

public interface ISubscriptionRepository
{
    /// <summary>
    /// Получить запись подписки по id пользователя - подписчика и того, на кого подписались
    /// </summary>
    /// <param name="followerId">Пользователь, кто подписался</param>
    /// <param name="followingId">Пользователь, на кого подписались</param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>Модель подписки с параметрами</returns>
    Task<SubscriptionDomainModel?> GetByFollowerAndFollowingAsync(int followerId, int followingId, CancellationToken ct = default);

    /// <summary>
    /// Создать запись подписки
    /// </summary>
    /// <param name="followerId"></param>
    /// <param name="followingId"></param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>Void</returns>
    Task AddAsync(int followerId, int followingId, CancellationToken ct = default);
    
    /// <summary>
    /// Получить запись подписки по id подписки
    /// </summary>
    /// <param name="id">Id записи подписки</param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>Dto модель подписки с ее параметрами</returns>
    Task<SubscriptionDomainModel?> GetByIdAsync(int id, CancellationToken ct = default);
    
    /// <summary>
    /// Получить все подписки пользователя
    /// </summary>
    /// <param name="follower">Пользователь, который подписался</param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>Список Dto моделей пользователей с данными о параметрах подписки</returns>
    Task<List<SubscriptionWithFollowingProfileDto>> GetListSubscriptionFollowersByFollowerAsync(int follower, CancellationToken ct = default);
    
    /// <summary>
    /// Получить список подписчиков по id пользователя, на которого они подписаны
    /// </summary>
    /// <param name="followingId">На кого подписались</param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>Список Dto моделей пользователей с данными о параметрах подписки.</returns>
    Task<List<SubscriptionWithFollowerProfileDto>> GetListSubscriptionFollowingAsync(int followingId, CancellationToken ct = default);

    /// <summary>
    /// Получить список id профилей пользователей, кто подписан на пользователя, у кого включены уведомления
    /// </summary>
    /// <param name="followingId">UserProfile на кого подписаны</param>
    /// <param name="ct"></param>
    /// <returns>List of UserProfile Ids</returns>
    public Task<List<int>> GetUserIdsWithActiveNotification(int followingId, CancellationToken ct = default);
    
    /// <summary>
    /// Сохранить 
    /// </summary>
    /// <param name="subscriptionModel">Доменная сущность</param>
    /// <param name="ct">Оборвать запрос</param>
    /// <returns>void</returns>
    Task SaveChangesAsync(CancellationToken ct = default);
}