using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Comment.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.CommentReaction;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Comment.Handlers;

public class SetCommentReactionCommandHandler : IRequestHandler<SetCommentReactionCommand, ReactionResultDto>
{
    private readonly ILogger<SetCommentReactionCommandHandler> _logger;
    private readonly ICommentReactionRepository _commentReactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public SetCommentReactionCommandHandler(ILogger<SetCommentReactionCommandHandler> logger,
        ICommentReactionRepository commentReactionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _commentReactionRepository = commentReactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReactionResultDto> Handle(SetCommentReactionCommand request, CancellationToken cancellationToken)
    {
        var existingReaction = await _commentReactionRepository.GetByCommentAndUserAsync(
            request.CommentId,
            request.UserId,
            cancellationToken);
        
        if (existingReaction != null)
        {
            existingReaction.UpdateReaction(request.ReactionType);
            await _commentReactionRepository.UpdateAsync(existingReaction, cancellationToken);
        }
        else
        {
            existingReaction = new CommentReaction(request.CommentId, request.UserId, request.ReactionType);
            await _commentReactionRepository.AddAsync(existingReaction, cancellationToken);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var likesCount = await _commentReactionRepository.GetCountByReactionTypeAsync(
            request.CommentId,
            ReactionType.Like,
            cancellationToken);
        var dislikesCount = await _commentReactionRepository.GetCountByReactionTypeAsync(
            request.CommentId,
            ReactionType.Dislike,
            cancellationToken);
        
        return new ReactionResultDto(
            request.CommentId,
            likesCount,
            dislikesCount,
            existingReaction.ReactionType);
    }
}