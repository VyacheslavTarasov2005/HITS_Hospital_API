using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IPatientsRepository _patientsRepository;
    private readonly IInspectionsRepository _inspectionsRepository;

    public PatientsService(IPatientsRepository patientsRepository, IInspectionsRepository inspectionsRepository)
    {
        _patientsRepository = patientsRepository;
        _inspectionsRepository = inspectionsRepository;
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

    public async Task<(List<Patient>?, Pagination)> GetPatients(String? name, List<Conclusion>? conclusions, 
        Sorting? sorting, bool scheduledVisits, Guid? doctorId, int page, int size)
    {
        var patients = await _patientsRepository.GetAllByNamePart(name ?? "");
        
        var inspections = await _inspectionsRepository.GetAll();
        
        if (conclusions != null || scheduledVisits || doctorId != null || sorting == Sorting.InspectionAsc ||
            sorting == Sorting.InspectionDesc)
        {
            patients = patients.Where(p => inspections
                    .Any(i => i.PatientId == p.Id &&
                              (conclusions == null || conclusions.Contains(i.Conclusion)) &&
                              (!scheduledVisits || i.NextVisitDate > DateTime.UtcNow) &&
                              (doctorId == null || i.DoctorId == doctorId)))
                .ToList();
        }
        
        Pagination pagination = new Pagination(size, patients.Count, page);

        if (patients.Count == 0)
        {
            return (patients, pagination);
        }

        switch (sorting)
        {
            case Sorting.NameAsc:
                patients = patients.OrderBy(p => p.Name).ToList();
                break;
            
            case Sorting.NameDesc:
                patients = patients.OrderByDescending(p => p.Name).ToList();
                break;
            
            case Sorting.CreateAsc:
                patients = patients.OrderBy(p => p.CreateTime).ToList();
                break;
            
            case Sorting.CreateDesc:
                patients = patients.OrderByDescending(p => p.CreateTime).ToList();
                break;
            
            case Sorting.InspectionAsc:
                patients = patients.OrderBy(p => inspections
                    .Where(i => i.PatientId == p.Id)
                    .Min(i => i.Date)).ToList();
                break;
            
            case Sorting.InspectionDesc:
                patients = patients.OrderByDescending(p => inspections
                    .Where(i => i.PatientId == p.Id)
                    .Max(i => i.Date)).ToList();
                break;
        }
        
        if (size * (page - 1) + 1 > patients.Count)
        {
            return (null, pagination);
        }
        
        List<Patient> patientsPaginated = new List<Patient>();
        
        for (int i = size * (page - 1); i < int.Min(size * page, patients.Count); i++)
        {
            patientsPaginated.Add(patients[i]);
        }
        
        return (patientsPaginated, pagination);
    }
}