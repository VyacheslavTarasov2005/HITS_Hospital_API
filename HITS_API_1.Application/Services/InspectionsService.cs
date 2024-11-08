using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class InspectionsService(
    IInspectionsRepository inspectionsRepository,
    IDiagnosesRepository diagnosesRepository,
    IConsultationsService consultationsService,
    IPatientsRepository patientsRepository,
    IDoctorsRepository doctorsRepository,
    IDiagnosesService diagnosesService,
    IIcd10Repository icd10Repository,
    IConsultationsRepository consultationsRepository,
    IPaginationService paginationService)
    : IInspectionsService
{
    public async Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId)
    {
        var patient = await patientsRepository.GetById(patientId);

        if (patient == null)
        {
            throw new NullReferenceException("Пациента не существует");
        }
        
        Inspection inspection = new Inspection(request.date, request.anamnesis, request.complaints, request.treatment, 
            request.conclusion, request.nextVisitDate, request.deathDate, request.previousInspectionId, 
            patientId, doctorId);
        
        var patientInspections = await inspectionsRepository.GetAllByPatientId(patientId);

        foreach (var patientInspection in patientInspections)
        {
            if (patientInspection.Conclusion == Conclusion.Death)
            {
                if (inspection.Conclusion == Conclusion.Death)
                {
                    throw new ArgumentException("Пациент уже умер");
                }
                
                if (patientInspection.Date < inspection.Date)
                {
                    throw new ArgumentException("Нельзя посавить дату осмотра позже даты смерти пациента");
                }
            }

            if (request.conclusion == Conclusion.Death && request.deathDate < patientInspection.Date)
            {
                throw new ArgumentException("Дата смерти не может быть раньше даты какого-либо другого осмотра");
            }
        }
        
        await inspectionsRepository.Create(inspection);

        foreach (var diagnosis in request.diagnoses)
        {
            Diagnosis item = new Diagnosis(diagnosis.description, diagnosis.type, inspection.Id, 
                diagnosis.icdDiagnosisId);
            
            await diagnosesRepository.Create(item);
        }
        
        if (request.consultations != null && request.consultations.Count > 0)
        {
            foreach (var consultation in request.consultations)
            {
                await consultationsService.CreateConsultation(inspection.Id, consultation.specialityId, 
                    inspection.DoctorId, consultation.comment.content);
            }
        }
        
        return inspection.Id;
    }

    public async Task<Inspection?> GetInspectionById(Guid inspectionId)
    {
        var inspection = await inspectionsRepository.GetById(inspectionId);

        return inspection;
    }

    public async Task<Inspection?> GetBaseInspection(Inspection inspection)
    {
        if (inspection.PreviousInspectionId == null)
        {
            return null;
        }
        
        var parentInspection = await inspectionsRepository.GetById(inspection.PreviousInspectionId.Value);

        while (parentInspection?.PreviousInspectionId != null)
        {
            parentInspection = await inspectionsRepository.GetById(parentInspection.PreviousInspectionId.Value);
        }

        return parentInspection;
    }

    public async Task UpdateInspection(RedactInspectionRequest request, Inspection inspection)
    {
        if (request.nextVisitDate != null)
        {
            if (inspection.Date > request.nextVisitDate)
            {
                throw new ArgumentException("Дата следующего визита не может быть раньше даты осмотра");
            }
        }
        
        var patientInspections = await inspectionsRepository.GetAllByPatientId(inspection.PatientId);
        
        foreach (var patientInspection in patientInspections)
        {
            if (patientInspection.Id != inspection.Id)
            {
                if (patientInspection.Conclusion == Conclusion.Death)
                {
                    if (request.conclusion == Conclusion.Death)
                    {
                        throw new ArgumentException("Пациент уже умер");
                    }
                }
                
                if (request.conclusion == Conclusion.Death && request.deathDate < patientInspection.Date)
                {
                    throw new ArgumentException("Дата смерти не может быть раньше даты какого-либо другого осмотра");
                }
            }
            else if (request.deathDate != null && request.deathDate > patientInspection.Date)
            {
                throw new ArgumentException("Дата смерти не может быть позже даты осмотра");
            }
        }
        
        await inspectionsRepository.Update(inspection.Id, request.anamnesis, request.complaints, request.treatment,
            request.conclusion, request.nextVisitDate, request.deathDate);
        
        await diagnosesRepository.DeleteByInspectionId(inspection.Id);

        foreach (var diagnosisRequest in request.diagnoses)
        {
            Diagnosis diagnosis = new Diagnosis(diagnosisRequest.description, diagnosisRequest.type, inspection.Id,
                diagnosisRequest.icdDiagnosisId);

            await diagnosesRepository.Create(diagnosis);
        }
    }

    public async Task<List<GetInspectionByRootResponse>> GetInspectionsByRoot(Guid rootId)
    {
        var rootInspection = await inspectionsRepository.GetById(rootId);

        if (rootInspection == null)
        {
            throw new NullReferenceException("Корневой осмотр не найден");
        }

        if (rootInspection.PreviousInspectionId != null)
        {
            throw new ArgumentException("Осмотр не является корневым");
        }
        
        List<GetInspectionByRootResponse> children = new List<GetInspectionByRootResponse>();
        
        var child = await inspectionsRepository.GetByParentInspectionId(rootInspection.Id);

        while (child != null)
        {
            var doctor = await doctorsRepository.GetById(child.DoctorId);
            var patient = await patientsRepository.GetById(child.PatientId);

            var diagnoses = await diagnosesService.GetDiagnosesByInspection(child.Id);
            
            var mainDiagnosis = diagnoses.FirstOrDefault(d => d.diagnosisType == DiagnosisType.Main);
            
            var nextChild = await inspectionsRepository.GetByParentInspectionId(child.Id);
            
            bool hasNested = nextChild != null;

            bool hasChain = !(!hasNested && child.PreviousInspectionId == null);

            GetInspectionByRootResponse childResponse = new GetInspectionByRootResponse(child.Id, child.CreateTime,
                child.PreviousInspectionId, child.Date, child.Conclusion, child.DoctorId, doctor.Name, child.PatientId,
                patient.Name, mainDiagnosis, hasChain, hasNested);
            
            children.Add(childResponse);
            
            child = nextChild;
        }

        return children;
    }

    public async Task<List<GetPatientInspectionsNoChildrenResponse>?> GetPatientInspectionsNoChildren(
        Guid patientId, String? filter)
    {
        var inspections = await inspectionsRepository.GetAllByPatientId(patientId);

        if (inspections.Count == 0)
        {
            return null;
        }
        
        inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        
        List<GetPatientInspectionsNoChildrenResponse> response = new List<GetPatientInspectionsNoChildrenResponse>();

        foreach (var inspection in inspections)
        {
            var diagnoses = await diagnosesService.GetDiagnosesByInspection(inspection.Id);
            
            var mainDiagnosis = diagnoses
                .FirstOrDefault(d => d.diagnosisType == DiagnosisType.Main);

            if (filter == null)
            {
                filter = "";
            }

            if (mainDiagnosis.code.ToLower().Contains(filter.ToLower()) ||
                mainDiagnosis.name.ToLower().Contains(filter.ToLower()))
            {
                GetPatientInspectionsNoChildrenResponse responseElement = new GetPatientInspectionsNoChildrenResponse(
                    inspection.Id, inspection.CreateTime, inspection.Date, mainDiagnosis);
                
                response.Add(responseElement);
            }
        }
        
        return response;
    }

    public async Task<(List<GetInspectionByRootResponse>, Pagination)> GetInspectionsForConsultation(Doctor doctor, 
        bool? grouped, List<Guid>? icdRoots, int? page, int? size)
    {
        var inspections = await inspectionsRepository.GetAll();
        
        var consultations = await consultationsRepository.GetAll();
        
        inspections = inspections.Where(i => consultations
                .Any(c => c.InspectionId == i.Id && c.SpecialityId == doctor.Speciality))
            .ToList();
        
        var response = await FilterAndPaginateInspections(grouped, icdRoots, page, 
            size, inspections);

        return response;
    }

    public async Task<(List<GetInspectionByRootResponse>, Pagination)> GetPatientInspections(Patient patient,
        bool? grouped, List<Guid>? icdRoots, int? page, int? size)
    {
        var inspections = await inspectionsRepository.GetAllByPatientId(patient.Id);
        
        var response = await FilterAndPaginateInspections(grouped, icdRoots,
            page, size, inspections);

        return response;
    }

    private async Task<(List<GetInspectionByRootResponse>, Pagination)> FilterAndPaginateInspections(bool? grouped,
        List<Guid>? icdRoots, int? page, int? size, List<Inspection> inspections)
    {
        grouped ??= false;
        icdRoots ??= new List<Guid>();
        
        if (grouped.Value)
        {
            inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        }
        
        List<GetInspectionByRootResponse> response = new List<GetInspectionByRootResponse>();
        
        foreach (var inspection in inspections)
        {
            var diagnoses = await diagnosesRepository.GetAllByInspection(inspection.Id);
            var mainDiagnosis = diagnoses
                .FirstOrDefault(d => d.Type == DiagnosisType.Main);

            if (mainDiagnosis == null)
            {
                continue;
            }

            if (icdRoots != null && icdRoots.Count > 0 && !icdRoots.Contains(mainDiagnosis.Icd10Id))
            {
                var mainDiagnosisIcdRoot = await icd10Repository.GetRootByChildId(mainDiagnosis.Icd10Id);

                if (mainDiagnosisIcdRoot == null || !icdRoots.Contains(mainDiagnosisIcdRoot.Id))
                {
                    continue;
                }
            }
            
            var author = await doctorsRepository.GetById(inspection.DoctorId);
            var patient = await patientsRepository.GetById(inspection.PatientId);
                
            var child = await inspectionsRepository.GetByParentInspectionId(inspection.Id);
            
            bool hasNested = child != null;
                
            bool hasChain = !(!hasNested && inspection.PreviousInspectionId == null);

            Icd10Entity icd = await icd10Repository.GetById(mainDiagnosis.Icd10Id);

            GetDiagnosisResponse diagnosisResponse = new GetDiagnosisResponse(mainDiagnosis.Id,
                mainDiagnosis.CreateTime, icd.Code, icd.Name, mainDiagnosis.Description, mainDiagnosis.Type);
                
            GetInspectionByRootResponse inspectionResponse = new GetInspectionByRootResponse(inspection.Id, 
                inspection.CreateTime, inspection.PreviousInspectionId, inspection.Date, inspection.Conclusion,
                author.Id, author.Name, patient.Id, patient.Name, diagnosisResponse, hasChain, hasNested);
                
            response.Add(inspectionResponse);
        }

        return paginationService.PaginateList(response, page, size);
    }
}