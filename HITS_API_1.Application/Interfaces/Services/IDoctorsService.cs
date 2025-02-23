using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IDoctorsService
{
    Task<String> RegisterDoctor(RegistrationRequest request);
    Task<String> LoginDoctor(LoginRequest request);
    Task LogoutDoctor(String token);
    Task<Doctor> GetDoctor(Guid id);

    Task UpdateDoctor(Guid id, UpdateDoctorRequest request);
}