using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IPatientsRepository
{
    Task<Guid> Create(Patient patient);
    Task<Patient?> GetById(Guid patientId);
    Task<List<Patient>> GetAll();
    Task<List<Patient>> GetAllByNamePart(String name);
}