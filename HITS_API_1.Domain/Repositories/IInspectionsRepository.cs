using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IInspectionsRepository
{
    Task<Guid> Create(Inspection inspection);
    Task<Inspection?> GetById(Guid id);
}