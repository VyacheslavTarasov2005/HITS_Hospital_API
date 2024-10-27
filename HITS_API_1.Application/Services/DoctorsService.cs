using HITS_API_1.Application.Interfaces;
using HITS_API_1.Domain;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DoctorsService : IDoctorsService
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly ITokensService _tokensService;
    private readonly IHasher _hasher;

    public DoctorsService(
        IDoctorsRepository doctorsRepository,
        ITokensService tokensService, 
        IHasher hasher)
    {
        _doctorsRepository = doctorsRepository;
        _tokensService = tokensService;
        _hasher = hasher;
    }

    public async Task<String> RegisterDoctor(Doctor doctor)
    {
        doctor.Password = _hasher.Hash(doctor.Password);
        var doctorId = await _doctorsRepository.Create(doctor);
        
        return await _tokensService.CreateToken(doctorId);
    }

    public async Task<String?> LoginDoctor(String email, String password)
    {
        var doctor = await _doctorsRepository.GetByEmail(email);
        if (doctor == null)
        {
            return null;
        }

        if (_hasher.Verify(password, doctor.Password))
        {
            return await _tokensService.CreateToken(doctor.Id);
        }
        
        return null;
    }

    public async Task<Doctor?> GetDoctor(Guid id)
    {
        return await _doctorsRepository.GetById(id);
    }

    public async Task<Guid> UpdateDoctor(Guid id, String email, String name, DateTime birthday, Gender gender,
        String phoneNumber)
    {
        return await _doctorsRepository.Update(id, email, name, birthday, gender, phoneNumber);
    }
}