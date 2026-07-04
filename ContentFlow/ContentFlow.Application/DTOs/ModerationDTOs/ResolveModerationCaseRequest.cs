using System.ComponentModel.DataAnnotations;
using ContentFlow.Domain.Enums;

namespace ContentFlow.Application.DTOs.ModerationDTOs;

public record ResolveModerationCaseRequest(
    [Required] ModerationDecision Decision,
    string? Note);
