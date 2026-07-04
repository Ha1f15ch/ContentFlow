using System.ComponentModel.DataAnnotations;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ReportDTOs;

public record CreateReportRequest(
    [Required] ReportReasonType ReasonType,
    string? Description);
