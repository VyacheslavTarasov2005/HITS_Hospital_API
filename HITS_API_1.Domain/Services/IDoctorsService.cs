using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain;

public interface IDoctorsService
{
    Task<String> RegisterDoctor(Doctor doctor);
    Task<Doctor?> GetDoctor(Guid id);

    Task<Guid> UpdateDoctor(Guid id, String email, String name, DateTime birthday, Gender gender,
        String phoneNumber);
}