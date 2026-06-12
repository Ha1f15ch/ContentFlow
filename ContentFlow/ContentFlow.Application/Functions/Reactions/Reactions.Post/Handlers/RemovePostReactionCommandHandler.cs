using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Post.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.PostReaction;
using ContentFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Post.Handlers;

public class RemovePostReactionCommandHandler : IRequestHandler<RemovePostReactionCommand, ReactionResultDto>
{
    private readonly ILogger<RemovePostReactionCommandHandler> _logger;
    private readonly IPostReactionRepository _postReactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePostReactionCommandHandler(
        ILogger<RemovePostReactionCommandHandler> logger,
        IPostReactionRepository postReactionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _postReactionRepository = postReactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReactionResultDto> Handle(
        RemovePostReactionCommand request,
        CancellationToken cancellationToken)
    {
        var existingReaction = await _postReactionRepository.GetByPostAndUserAsync(
            request.PostId,
            request.UserId,
            cancellationToken);

        if (existingReaction is not null)
        {
            await _postReactionRepository.DeleteAsync(existingReaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var likesCount = await _postReactionRepository.GetCountByReactionTypeAsync(
            request.PostId,
            ReactionType.Like,
            cancellationToken);

        var dislikesCount = await _postReactionRepository.GetCountByReactionTypeAsync(
            request.PostId,
            ReactionType.Dislike,
            cancellationToken);

        return new ReactionResultDto(
            request.PostId,
            likesCount,
            dislikesCount,
            null);
    }
}