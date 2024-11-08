using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class PatientsService(
    IPatientsRepository patientsRepository,
    IInspectionsRepository inspectionsRepository,
    IDiagnosesRepository diagnosesRepository,
    IIcd10Repository icd10Repository,
    IPaginationService paginationService)
    : IPatientsService
{
    public async Task<Guid> CreatePatient(String name, DateTime? birthday, Gender gender)
    {
        Patient patient = new Patient(name, birthday, gender);
        return await patientsRepository.Create(patient);
    }

    public async Task<Patient?> GetPatientById(Guid id)
    {
        var patient = await patientsRepository.GetById(id);
        
        return patient;
    }

    public async Task<(List<Patient>, Pagination)> GetPatients(String? name, List<Conclusion>? conclusions, 
        Sorting? sorting, bool scheduledVisits, Guid? doctorId, int? page, int? size)
    {
        var patients = await patientsRepository.GetAllByNamePart(name ?? "");
        
        var inspections = await inspectionsRepository.GetAll();
        
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

        return paginationService.PaginateList(patients, page, size);
    }

    public async Task<GetReportResponse> GetPatientsReport(GetReportRequest request)
    {
        var patients = await patientsRepository.GetAllByNamePart("");
        
        Dictionary<String, int> icdCounter = new Dictionary<String, int>();
        List<GetReportRecordResponse> records = new List<GetReportRecordResponse>();

        foreach (var patient in patients)
        {
            var inspections = await inspectionsRepository.GetAllByPatientId(patient.Id);
            Dictionary<String, int> patientIcdCounter = new Dictionary<String, int>();

            foreach (var inspection in inspections)
            {
                var diagnoses = await diagnosesRepository.GetAllByInspection(inspection.Id);

                var mainDiagnosis = diagnoses.FirstOrDefault(d => d.Type == DiagnosisType.Main);

                if (mainDiagnosis == null)
                {
                    continue;
                }
                
                var mainDiagnosisIcdRoot = await icd10Repository.GetRootByChildId(mainDiagnosis.Icd10Id);

                if (request.icdRoots != null && request.icdRoots.Count > 0 &&
                    !request.icdRoots.Contains(mainDiagnosis.Icd10Id))
                {
                    if (mainDiagnosisIcdRoot != null && request.icdRoots.Contains(mainDiagnosisIcdRoot.Id))
                    {
                        // Обновление количества у пациента
                        if (!patientIcdCounter.ContainsKey(mainDiagnosisIcdRoot.Code))
                        {
                            patientIcdCounter[mainDiagnosisIcdRoot.Code] = 1;
                        }
                        else
                        {
                            patientIcdCounter[mainDiagnosisIcdRoot.Code]++;
                        }
                        
                        // Обновление общего количества
                        if (!icdCounter.ContainsKey(mainDiagnosisIcdRoot.Code))
                        {
                            icdCounter[mainDiagnosisIcdRoot.Code] = 1;
                        }
                        else
                        {
                            icdCounter[mainDiagnosisIcdRoot.Code]++;
                        }
                    }
                }
                else
                {
                    // Обновление количества у пациента
                    if (!patientIcdCounter.ContainsKey(mainDiagnosisIcdRoot.Code))
                    {
                        patientIcdCounter[mainDiagnosisIcdRoot.Code] = 1;
                    }
                    else
                    {
                        patientIcdCounter[mainDiagnosisIcdRoot.Code]++;
                    }
                    
                    // Обновление общего количества
                    if (!icdCounter.ContainsKey(mainDiagnosisIcdRoot.Code))
                    {
                        icdCounter[mainDiagnosisIcdRoot.Code] = 1;
                    }
                    else
                    {
                        icdCounter[mainDiagnosisIcdRoot.Code]++;
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
            var roots = await icd10Repository.GetRoots();

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
                var icd = await icd10Repository.GetById(id);
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