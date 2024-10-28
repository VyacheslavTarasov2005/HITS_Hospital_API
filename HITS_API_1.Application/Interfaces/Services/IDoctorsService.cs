using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IDoctorsService
{
    Task<String> RegisterDoctor(Doctor doctor);
    Task<String?> LoginDoctor(String email, String password);
    Task LogoutDoctor(String token);
    Task<Doctor?> GetDoctor(Guid id);

    Task UpdateDoctor(Guid id, String email, String name, DateTime? birthday, Gender gender,
        String? phoneNumber);
}