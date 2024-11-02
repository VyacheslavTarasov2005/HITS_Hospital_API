using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IInspectionsRepository
{
    Task<Guid> Create(Inspection inspection);
    Task<Inspection?> GetById(Guid id);
    Task<Inspection?> GetByParentInspectionId(Guid parentInspectionId);
    Task<List<Inspection>> GetAllByPatientId(Guid patientId);

    Task Update(Guid id, String anamnesis, String complaints, String treatment, Conclusion conclusion,
        DateTime? nextVisitDate, DateTime? deathDate);
}