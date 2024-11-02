using HITS_API_1.Application.DTOs;
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

    public InspectionsService(
        IInspectionsRepository inspectionsRepository,
        IDiagnosesRepository diagnosesRepository,
        IConsultationsService consultationsService,
        IPatientsRepository patientsRepository,
        IDoctorsRepository doctorsRepository,
        IDiagnosesService diagnosesService)
    {
        _inspectionsRepository = inspectionsRepository;
        _diagnosesRepository = diagnosesRepository;
        _consultationsService = consultationsService;
        _patientsRepository = patientsRepository;
        _doctorsRepository = doctorsRepository;
        _diagnosesService = diagnosesService;
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

            GetInspectionByRootResponse childResponse = new GetInspectionByRootResponse(child.Id, child.CreateTime,
                child.PreviousInspectionId, child.Date, child.Conclusion, child.DoctorId, doctor.Name, child.PatientId,
                patient.Name, mainDiagnosis, true, hasNested);
            
            children.Add(childResponse);
            
            child = nextChild;
        }

        return children;
    }
}