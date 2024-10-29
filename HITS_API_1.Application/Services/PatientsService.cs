using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IPatientsRepository _patientsRepository;

    public PatientsService(IPatientsRepository patientsRepository)
    {
        _patientsRepository = patientsRepository;
    }

    public async Task<Guid> CreatePatient(String name, DateTime? birthday, Gender gender)
    {
        Patient patient = new Patient(name, birthday, gender);
        return await _patientsRepository.Create(patient);
    }

    public async Task<Patient?> GetPatientById(Guid id)
    {
        var patient = await _patientsRepository.GetById(id);
        
        return patient;
    }
}