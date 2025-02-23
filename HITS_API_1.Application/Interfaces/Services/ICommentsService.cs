using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Interfaces.Services;

public interface ICommentsService
{
    Task<Guid> CreateComment(Guid consultationId, AddCommentRequest request, Guid authorId);
    Task RedactComment(Guid commentId, RedactCommentRequest request, Guid authorId);
}