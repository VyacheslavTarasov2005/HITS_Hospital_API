using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IDoctorsRepository
{
    Task<Guid> Create(Doctor doctor);
    Task<Doctor?> Get(Guid id);
    Task<Guid> Update(Guid id, String email, String name, DateTime birthday, Gender gender, String phoneNumber);
}