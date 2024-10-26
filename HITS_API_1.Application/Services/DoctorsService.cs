using HITS_API_1.Domain;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DoctorsService : IDoctorsService
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly ITokensService _tokensService;

    public DoctorsService(IDoctorsRepository doctorsRepository, ITokensService tokensService)
    {
        _doctorsRepository = doctorsRepository;
        _tokensService = tokensService;
    }

    public async Task<String> CreateDoctor(Doctor doctor)
    {
        var doctorId = await _doctorsRepository.Create(doctor);
        
        return await _tokensService.CreateToken(doctorId);
    }

    public async Task<Doctor?> GetDoctor(Guid id)
    {
        return await _doctorsRepository.Get(id);
    }

    public async Task<Guid> UpdateDoctor(Guid id, String email, String name, DateTime birthday, Gender gender,
        String phoneNumber)
    {
        return await _doctorsRepository.Update(id, email, name, birthday, gender, phoneNumber);
    }
}