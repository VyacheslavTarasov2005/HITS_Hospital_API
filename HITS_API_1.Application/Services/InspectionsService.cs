using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class InspectionsService : IInspectionsService
{
    private readonly IInspectionsRepository _inspectionsRepository;
    private readonly IDiagnosesRepository _diagnosesRepository;
    private readonly IConsultationsService _consultationsService;
    private readonly IPatientsRepository _patientsRepository;
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly IDiagnosesService _diagnosesService;
    private readonly IIcd10Repository _icd10Repository;
    private readonly IConsultationsRepository _consultationsRepository;

    public InspectionsService(
        IInspectionsRepository inspectionsRepository,
        IDiagnosesRepository diagnosesRepository,
        IConsultationsService consultationsService,
        IPatientsRepository patientsRepository,
        IDoctorsRepository doctorsRepository,
        IDiagnosesService diagnosesService,
        IIcd10Repository icd10Repository,
        IConsultationsRepository consultationsRepository)
    {
        _inspectionsRepository = inspectionsRepository;
        _diagnosesRepository = diagnosesRepository;
        _consultationsService = consultationsService;
        _patientsRepository = patientsRepository;
        _doctorsRepository = doctorsRepository;
        _diagnosesService = diagnosesService;
        _icd10Repository = icd10Repository;
        _consultationsRepository = consultationsRepository;
    }

    public async Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId)
    {
        var patient = await _patientsRepository.GetById(patientId);

        if (patient == null)
        {
            throw new ArgumentException("Пациента не существует");
        }
        
        Inspection inspection = new Inspection(request.date, request.anamnesis, request.complaints, request.treatment, 
            request.conclusion, request.nextVisitDate, request.deathDate, request.previousInspectionId, 
            patientId, doctorId);
        
        var patientInspections = await _inspectionsRepository.GetAllByPatientId(patientId);

        foreach (var patientInspection in patientInspections)
        {
            if (patientInspection.Conclusion == Conclusion.Death)
            {
                if (patientInspection.Date < inspection.Date)
                {
                    throw new ArgumentException("Нельзя посавить дату осмотра позже даты смерти пациента");
                }

                if (inspection.Conclusion == Conclusion.Death)
                {
                    throw new ArgumentException("Пациент уже умер");
                }
            }
        }
        
        await _inspectionsRepository.Create(inspection);

        foreach (var diagnosis in request.diagnoses)
        {
            Diagnosis item = new Diagnosis(diagnosis.description, diagnosis.type, inspection.Id, 
                diagnosis.icdDiagnosisId);
            
            await _diagnosesRepository.Create(item);
        }
        
        if (request.consultations != null && request.consultations.Count > 0)
        {
            foreach (var consultation in request.consultations)
            {
                await _consultationsService.CreateConsultation(inspection.Id, consultation.specialityId, 
                    inspection.DoctorId, consultation.comment.content);
            }
        }
        
        return inspection.Id;
    }

    public async Task<Inspection?> GetInspectionById(Guid inspectionId)
    {
        var inspection = await _inspectionsRepository.GetById(inspectionId);

        return inspection;
    }

    public async Task<Inspection?> GetBaseInspection(Inspection inspection)
    {
        if (inspection.PreviousInspectionId == null)
        {
            return null;
        }
        
        var parentInspection = await _inspectionsRepository.GetById(inspection.PreviousInspectionId.Value);

        while (parentInspection?.PreviousInspectionId != null)
        {
            parentInspection = await _inspectionsRepository.GetById(parentInspection.PreviousInspectionId.Value);
        }

        return parentInspection;
    }

    public async Task UpdateInspection(RedactInspectionRequest request, Guid id)
    {
        await _inspectionsRepository.Update(id, request.anamnesis, request.complaints, request.treatment,
            request.conclusion, request.nextVisitDate, request.deathDate);
        
        await _diagnosesRepository.DeleteByInspectionId(id);

        foreach (var diagnosisRequest in request.diagnoses)
        {
            Diagnosis diagnosis = new Diagnosis(diagnosisRequest.description, diagnosisRequest.type, id,
                diagnosisRequest.icdDiagnosisId);

            await _diagnosesRepository.Create(diagnosis);
        }
    }

    public async Task<List<GetInspectionByRootResponse>?> GetInspectionsByRoot(Guid rootId)
    {
        var rootInspection = await _inspectionsRepository.GetById(rootId);

        if (rootInspection == null || rootInspection.PreviousInspectionId != null)
        {
            return null;
        }
        
        List<GetInspectionByRootResponse> children = new List<GetInspectionByRootResponse>();
        
        var child = await _inspectionsRepository.GetByParentInspectionId(rootInspection.Id);

        while (child != null)
        {
            var doctor = await _doctorsRepository.GetById(child.DoctorId);
            var patient = await _patientsRepository.GetById(child.PatientId);

            var diagnoses = await _diagnosesService.GetDiagnosesByInspection(child.Id);
            
            var mainDiagnosis = diagnoses.FirstOrDefault(d => d.diagnosisType == DiagnosisType.Main);
            
            var nextChild = await _inspectionsRepository.GetByParentInspectionId(child.Id);
            
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
        var inspections = await _inspectionsRepository.GetAllByPatientId(patientId);

        if (inspections.Count == 0)
        {
            return null;
        }
        
        inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        
        List<GetPatientInspectionsNoChildrenResponse> response = new List<GetPatientInspectionsNoChildrenResponse>();

        foreach (var inspection in inspections)
        {
            var diagnoses = await _diagnosesService.GetDiagnosesByInspection(inspection.Id);
            
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

    public async Task<(List<GetInspectionByRootResponse>?, Pagination)> GetInspectionsForConsultation(Doctor doctor, 
        bool? grouped, List<Guid>? icdRoots, int page, int size)
    {
        grouped ??= false;
        icdRoots ??= new List<Guid>();
        
        var inspections = await _inspectionsRepository.GetAll();

        if (grouped.Value)
        {
            inspections = inspections.Where(i => i.PreviousInspectionId == null).ToList();
        }

        var consultations = await _consultationsRepository.GetAll();
        
        inspections = inspections.Where(i => consultations
            .Any(c => c.InspectionId == i.Id && c.SpecialityId == doctor.Speciality))
            .ToList();

        List<GetInspectionByRootResponse> response = new List<GetInspectionByRootResponse>();
        
        foreach (var inspection in inspections)
        {
            var diagnoses = await _diagnosesRepository.GetAllByInspection(inspection.Id);
            var mainDiagnosis = diagnoses
                .FirstOrDefault(d => d.Type == DiagnosisType.Main);

            if (icdRoots != null && icdRoots.Count != 0 && !icdRoots.Contains(mainDiagnosis.Icd10Id))
            {
                continue;
            }
            
            var author = await _doctorsRepository.GetById(inspection.DoctorId);
            var patient = await _patientsRepository.GetById(inspection.PatientId);
                
            var child = await _inspectionsRepository.GetByParentInspectionId(inspection.Id);
            
            bool hasNested = child != null;
                
            bool hasChain = !(!hasNested && inspection.PreviousInspectionId == null);

            Icd10Entity icd = await _icd10Repository.GetById(mainDiagnosis.Icd10Id);

            GetDiagnosisResponse diagnosisResponse = new GetDiagnosisResponse(mainDiagnosis.Id,
                mainDiagnosis.CreateTime, icd.Code, icd.Name, mainDiagnosis.Description, mainDiagnosis.Type);
                
            GetInspectionByRootResponse inspectionResponse = new GetInspectionByRootResponse(inspection.Id, 
                inspection.CreateTime, inspection.PreviousInspectionId, inspection.Date, inspection.Conclusion,
                author.Id, author.Name, patient.Id, patient.Name, diagnosisResponse, hasChain, hasNested);
                
            response.Add(inspectionResponse);
        }

        Pagination pagination = new Pagination(size, response.Count, page);
        
        if (response.Count == 0)
        {
            return (response, pagination);
        }
        
        if (size * (page - 1) + 1 > response.Count)
        {
            return (null, pagination);
        }
        
        List<GetInspectionByRootResponse> responsePaginated = new List<GetInspectionByRootResponse>();
        
        for (int i = size * (page - 1); i < int.Min(size * page, response.Count); i++)
        {
            responsePaginated.Add(response[i]);
        }

        return (responsePaginated, pagination);
    }
}