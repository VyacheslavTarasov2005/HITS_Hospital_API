using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class PatientsService : IPatientsService
{
    private readonly IPatientsRepository _patientsRepository;
    private readonly IInspectionsRepository _inspectionsRepository;
    private readonly IDiagnosesRepository _diagnosesRepository;
    private readonly IIcd10Repository _icd10Repository;

    public PatientsService(
        IPatientsRepository patientsRepository, 
        IInspectionsRepository inspectionsRepository,
        IDiagnosesRepository diagnosesRepository,
        IIcd10Repository icd10Repository)
    {
        _patientsRepository = patientsRepository;
        _inspectionsRepository = inspectionsRepository;
        _diagnosesRepository = diagnosesRepository;
        _icd10Repository = icd10Repository;
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

    public async Task<GetReportResponse> GetPatientsReport(GetReportRequest request)
    {
        var patients = await _patientsRepository.GetAllByNamePart("");
        
        Dictionary<String, int> icdCounter = new Dictionary<String, int>();
        List<GetReportRecordResponse> records = new List<GetReportRecordResponse>();

        foreach (var patient in patients)
        {
            var inspections = await _inspectionsRepository.GetAllByPatientId(patient.Id);
            Dictionary<String, int> patientIcdCounter = new Dictionary<String, int>();

            foreach (var inspection in inspections)
            {
                var diagnoses = await _diagnosesRepository.GetAllByInspection(inspection.Id);

                var mainDiagnosis = diagnoses.FirstOrDefault(d => d.Type == DiagnosisType.Main);

                if (request.icdRoots != null && request.icdRoots.Count > 0 &&
                    !request.icdRoots.Contains(mainDiagnosis.Icd10Id))
                {
                    foreach (var icdRoot in request.icdRoots)
                    {
                        var children = await _icd10Repository.GetAllByRoot(icdRoot);

                        if (children.Any(i => i.Id == mainDiagnosis.Icd10Id))
                        {
                            var icdEntity = await _icd10Repository.GetById(icdRoot);
                            
                            // Обновление количества у пациента
                            if (!patientIcdCounter.ContainsKey(icdEntity.Code))
                            {
                                patientIcdCounter[icdEntity.Code] = 1;
                            }
                            else
                            {
                                patientIcdCounter[icdEntity.Code]++;
                            }
                            
                            // Обновление общего количества
                            if (!icdCounter.ContainsKey(icdEntity.Code))
                            {
                                icdCounter[icdEntity.Code] = 1;
                            }
                            else
                            {
                                icdCounter[icdEntity.Code]++;
                            }
                            
                            break;
                        }
                    }
                }
                else
                {
                    var icd = await _icd10Repository.GetById(mainDiagnosis.Icd10Id);
                    
                    // Обновление количества у пациента
                    if (!patientIcdCounter.ContainsKey(icd.Code))
                    {
                        patientIcdCounter[icd.Code] = 1;
                    }
                    else
                    {
                        patientIcdCounter[icd.Code]++;
                    }
                    
                    // Обновление общего количества
                    if (!icdCounter.ContainsKey(icd.Code))
                    {
                        icdCounter[icd.Code] = 1;
                    }
                    else
                    {
                        icdCounter[icd.Code]++;
                    }
                }
            }

            if (patientIcdCounter.Count > 0)
            {
                GetReportRecordResponse patientRecordResponse = new GetReportRecordResponse(patient.Name,
                    patient.Birthday, patient.Sex, patientIcdCounter);
                records.Add(patientRecordResponse);
            }
        }
        
        records = records.OrderBy(r=> r.patientName).ToList();
        
        GetReportFilterResponse filter;
        
        if (request.icdRoots == null || request.icdRoots.Count == 0)
        {
            var roots = await _icd10Repository.GetRoots();

            foreach (var icd in roots)
            {
                if (!icdCounter.ContainsKey(icd.Code))
                {
                    icdCounter[icd.Code] = 0;
                }
            }
            
            filter = new GetReportFilterResponse(request.start, request.end, roots
                .Select(i => i.Code)
                .Order()
                .ToList());
        }
        else
        {
            List<String> roots = new List<String>();

            foreach (var id in request.icdRoots)
            {
                var icd = await _icd10Repository.GetById(id);
                roots.Add(icd.Code);
                
                if (!icdCounter.ContainsKey(icd.Code))
                {
                    icdCounter[icd.Code] = 0;
                }
            }
            
            filter = new GetReportFilterResponse(request.start, request.end, roots.Order().ToList());
        }
        
        icdCounter = icdCounter
            .OrderBy(i => i.Key)
            .ToDictionary(i => i.Key, i => i.Value);
        
        GetReportResponse response = new GetReportResponse(filter, records, icdCounter);
        
        return response;
    }
}