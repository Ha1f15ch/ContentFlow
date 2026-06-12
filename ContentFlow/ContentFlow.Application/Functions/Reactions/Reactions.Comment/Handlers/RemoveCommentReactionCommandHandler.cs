using ContentFlow.Application.DTOs.ReactionDTOs;
using ContentFlow.Application.Functions.Reactions.Reactions.Comment.Commands;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.CommentReaction;
using ContentFlow.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Application.Functions.Reactions.Reactions.Comment.Handlers;

public class RemoveCommentReactionCommandHandler : IRequestHandler<RemoveCommentReactionCommand, ReactionResultDto>
{
    private readonly ILogger<RemoveCommentReactionCommandHandler> _logger;
    private readonly ICommentReactionRepository _commentReactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RemoveCommentReactionCommandHandler(ILogger<RemoveCommentReactionCommandHandler> logger, 
        ICommentReactionRepository commentReactionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _commentReactionRepository = commentReactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReactionResultDto> Handle(RemoveCommentReactionCommand request,
        CancellationToken cancellationToken)
    {
        var existingReaction = await _commentReactionRepository.GetByCommentAndUserAsync(
            request.CommentId,
            request.UserId,
            cancellationToken);
        
        if (existingReaction is not null)
        {
            await _commentReactionRepository.DeleteAsync(existingReaction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
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
            null);
    }
}