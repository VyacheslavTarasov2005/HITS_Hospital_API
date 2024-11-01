using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IDiagnosesRepository
{
    Task<Guid> Create(Diagnosis diagnosis);
    Task<List<Diagnosis>> GetAllByInspection(Guid inspectionId);
    Task DeleteByInspectionId(Guid inspectionId);
}