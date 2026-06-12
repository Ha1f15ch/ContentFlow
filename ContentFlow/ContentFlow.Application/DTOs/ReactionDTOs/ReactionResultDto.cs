using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ReactionDTOs;

public record ReactionResultDto(
    int EntityId,
    int LikesCount,
    int DislikesCount,
    ReactionType? CurrentUserReaction
);