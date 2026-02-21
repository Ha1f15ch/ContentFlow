namespace ContentFlow.Application.DTOs;

public record CreatePostRequest(
    string Title,
    string Content);