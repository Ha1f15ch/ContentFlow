using ContentFlow.Application.Common;
using ContentFlow.Application.Exceptions;
using ContentFlow.Application.Interfaces.Comment;
using ContentFlow.Application.Interfaces.Common;
using ContentFlow.Application.Interfaces.ModerationCase;
using ContentFlow.Application.Interfaces.Posts;
using ContentFlow.Application.Interfaces.Report;
using ContentFlow.Application.Interfaces.Users;
using ContentFlow.Domain.Entities;
using ContentFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ContentFlow.Infrastructure.Services;

public class ReportSubmissionService : IReportSubmissionService
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IModerationCaseRepository _moderationCaseRepository;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportSubmissionService> _logger;

    public ReportSubmissionService(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IReportRepository reportRepository,
        IModerationCaseRepository moderationCaseRepository,
        IUserService userService,
        IUnitOfWork unitOfWork,
        ILogger<ReportSubmissionService> logger)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _reportRepository = reportRepository;
        _moderationCaseRepository = moderationCaseRepository;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<int> SubmitForPostAsync(
        int reporterId,
        int postId,
        ReportReasonType reasonType,
        string? description,
        CancellationToken ct)
    {
        await EnsureReporterCanSubmitAsync(reporterId, ct);

        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post == null)
            throw new NotFoundException("Post not found");

        if (post.AuthorId == reporterId)
            throw new ArgumentException("You cannot report your own post.");

        var existingReport = await _reportRepository.GetByReporterAndPostAsync(reporterId, postId, ct);
        if (existingReport != null)
            throw new ArgumentException("You have already reported this post.");

        var report = Report.ForPost(reporterId, postId, reasonType, description);
        await _reportRepository.AddAsync(report, ct);

        var existingCase = await _moderationCaseRepository.GetOpenByPostIdAsync(postId, ct);
        ModerationCase moderationCase;
        if (existingCase == null)
        {
            moderationCase = ModerationCase.OpenForPost(postId);
            await _moderationCaseRepository.AddAsync(moderationCase, ct);
        }
        else
        {
            moderationCase = existingCase;
        }

        moderationCase.RegisterReport(reasonType, isUniqueReporter: true);

        if (moderationCase.ShouldAutoHideContent)
            post.HideForModerationReview();

        await _postRepository.UpdateAsync(post, ct);
        if (existingCase != null)
            await _moderationCaseRepository.UpdateAsync(moderationCase, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Report {ReportId} created for post {PostId} by user {ReporterId}. Case {CaseId}",
            report.Id,
            postId,
            reporterId,
            moderationCase.Id);

        return report.Id;
    }

    public async Task<int> SubmitForCommentAsync(
        int reporterId,
        int commentId,
        ReportReasonType reasonType,
        string? description,
        CancellationToken ct)
    {
        await EnsureReporterCanSubmitAsync(reporterId, ct);

        var comment = await _commentRepository.GetByIdAsync(commentId, ct);
        if (comment == null || comment.IsDeleted)
            throw new NotFoundException("Comment not found");

        if (comment.AuthorId == reporterId)
            throw new ArgumentException("You cannot report your own comment.");

        var existingReport = await _reportRepository.GetByReporterAndCommentAsync(reporterId, commentId, ct);
        if (existingReport != null)
            throw new ArgumentException("You have already reported this comment.");

        var report = Report.ForComment(reporterId, commentId, reasonType, description);
        await _reportRepository.AddAsync(report, ct);

        var existingCase = await _moderationCaseRepository.GetOpenByCommentIdAsync(commentId, ct);
        ModerationCase moderationCase;
        if (existingCase == null)
        {
            moderationCase = ModerationCase.OpenForComment(commentId);
            await _moderationCaseRepository.AddAsync(moderationCase, ct);
        }
        else
        {
            moderationCase = existingCase;
        }

        moderationCase.RegisterReport(reasonType, isUniqueReporter: true);

        if (moderationCase.ShouldAutoHideContent)
            comment.HideForModerationReview();

        await _commentRepository.UpdateAsync(comment, ct);
        if (existingCase != null)
            await _moderationCaseRepository.UpdateAsync(moderationCase, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Report {ReportId} created for comment {CommentId} by user {ReporterId}. Case {CaseId}",
            report.Id,
            commentId,
            reporterId,
            moderationCase.Id);

        return report.Id;
    }

    private async Task EnsureReporterCanSubmitAsync(int reporterId, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(reporterId, ct);
        if (user == null)
            throw new NotFoundException("User not found");

        var canReport = await _userService.IsInRoleAsync(reporterId, RoleConstants.User) ||
                        await _userService.IsInRoleAsync(reporterId, RoleConstants.ContentEditor) ||
                        await _userService.IsInRoleAsync(reporterId, RoleConstants.Moderator) ||
                        await _userService.IsInRoleAsync(reporterId, RoleConstants.Admin);

        if (!canReport)
            throw new UnauthorizedAccessException("You do not have permission to submit reports.");
    }
}
