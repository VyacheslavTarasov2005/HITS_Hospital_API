using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IDoctorsRepository
{
    Task<Guid> Create(Doctor doctor);
    Task<Doctor?> GetById(Guid id);
    Task<Doctor?> GetByEmail(String email);
    Task<List<Doctor>> GetAllByEmail(String email);
    Task<Guid> Update(Guid id, String email, String name, DateTime? birthday, Gender gender, String? phoneNumber);
}