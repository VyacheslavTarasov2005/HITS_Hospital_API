using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DoctorsService(
    IDoctorsRepository doctorsRepository,
    ITokensService tokensService,
    IHasher hasher)
    : IDoctorsService
{
    public async Task<String> RegisterDoctor(Doctor doctor)
    {
        doctor.Password = hasher.Hash(doctor.Password);
        var doctorId = await doctorsRepository.Create(doctor);
        
        return await tokensService.CreateToken(doctorId);
    }

    public async Task<String?> LoginDoctor(String email, String password)
    {
        var doctor = await doctorsRepository.GetByEmail(email);
        if (doctor == null)
        {
            return null;
        }

        if (hasher.Verify(password, doctor.Password))
        {
            return await tokensService.CreateToken(doctor.Id);
        }
        
        return null;
    }

    public async Task LogoutDoctor(String token)
    {
        await tokensService.DeleteToken(token);
    }

    public async Task<Doctor?> GetDoctor(Guid id)
    {
        return await doctorsRepository.GetById(id);
    }

    public async Task UpdateDoctor(Guid id, String email, String name, DateTime? birthday, Gender gender,
        String? phoneNumber)
    {
        var doctor = await doctorsRepository.GetById(id);

        if (doctor == null)
        {
            throw new Exception("Пользователь не найден");
        }

        var sameEmailDoctor = await doctorsRepository.GetByEmail(email);

        if (sameEmailDoctor == null || sameEmailDoctor.Id == id)
        {
            await doctorsRepository.Update(id, email, name, birthday, gender, phoneNumber);
            return;
        }
        
        throw new Exception("email уже использован");
    }
}