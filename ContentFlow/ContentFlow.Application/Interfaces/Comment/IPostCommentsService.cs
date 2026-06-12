using ContentFlow.Application.DTOs;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.Interfaces.Comment;

public interface IPostCommentsService
{
    public List<CommentDto> BuildCommentsTree(
        List<Domain.Entities.Comment> comments,
        Dictionary<int, string> userNames,
        Dictionary<int, int> likesCounts,
        Dictionary<int, int> dislikesCounts,
        Dictionary<int, ReactionType?> currentUserReactions);
}