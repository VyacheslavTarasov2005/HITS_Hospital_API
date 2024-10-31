using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface ICommentsRepository
{
    Task<Comment> Create(Comment comment);
    Task<List<Comment>> GetByConsultationId(Guid id);
}