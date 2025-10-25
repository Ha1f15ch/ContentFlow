using System.ComponentModel.DataAnnotations;

namespace ContentFlow.Application.DTOs;

public record CreateCommentRequest([Required] string Content, int? ParentCommentId = null);