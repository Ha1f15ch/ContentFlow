using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Post.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.PostReaction;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Post.Handlers;

public class SetPostReactionCommandHandler : IRequestHandler<SetPostReactionCommand, ReactionResultDto>
{
    private readonly ILogger<SetPostReactionCommandHandler> _logger;
    private readonly IPostReactionRepository _postReactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPostReactionCommandHandler(
        ILogger<SetPostReactionCommandHandler> logger,
        IPostReactionRepository postReactionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _postReactionRepository = postReactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReactionResultDto> Handle(
        SetPostReactionCommand request,
        CancellationToken cancellationToken)
    {
        var existingReaction = await _postReactionRepository.GetByPostAndUserAsync(
            request.PostId,
            request.UserId,
            cancellationToken);

        if (existingReaction is not null)
        {
            existingReaction.UpdateReaction(request.ReactionType);
            await _postReactionRepository.UpdateAsync(existingReaction, cancellationToken);
        }
        else
        {
            existingReaction = new PostReaction(request.PostId, request.UserId, request.ReactionType);
            await _postReactionRepository.AddAsync(existingReaction, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            existingReaction.ReactionType);
    }
}