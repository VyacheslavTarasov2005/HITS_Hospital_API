using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface ICommentsRepository
{
    Task Create(Comment comment);
    Task<List<Comment>> GetByConsultationId(Guid id);
    Task<Comment?> GetById(Guid id);
    Task Update(Guid id, String content);
}