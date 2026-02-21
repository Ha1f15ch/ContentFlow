using System.ComponentModel.DataAnnotations;

namespace ContentFlow.Application.DTOs;

public record UpdatePostModel(
    [Required][StringLength(200)] string Title,
    [Required] string Content,
    List<int> TagIds);