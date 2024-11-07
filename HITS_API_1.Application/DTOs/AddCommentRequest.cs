namespace HITS_API_1.Application.DTOs;

public record AddCommentRequest(
    String content,
    Guid parentId);