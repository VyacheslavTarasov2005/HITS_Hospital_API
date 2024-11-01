using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IConsultationsRepository
{
    Task<Guid> Create(Consultation consultation);
    Task<Consultation?> GetById(Guid id);
    Task<List<Consultation>> GetAllByInspectionId(Guid inspectionId);
}