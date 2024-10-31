namespace HITS_API_1.Application.DTOs;

public record GetCommentModel(
    Guid id,
    DateTime createTime,
    DateTime? modifiedDate,
    String content,
    Guid authorId,
    String author,
    Guid? parentId);