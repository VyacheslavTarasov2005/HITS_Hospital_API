using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IPatientsService
{
    Task<Guid> CreatePatient(String name, DateTime? birthday, Gender gender);
}