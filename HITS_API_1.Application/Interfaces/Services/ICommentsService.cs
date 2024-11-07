namespace HITS_API_1.Application.Interfaces.Services;

public interface ICommentsService
{
    Task<Guid> CreateComment(String content, Guid parentId, Guid consultationId, Guid authorId);
    Task RedactComment(Guid commentId, String content);
}