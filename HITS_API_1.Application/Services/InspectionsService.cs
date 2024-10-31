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

    public InspectionsService(
        IInspectionsRepository inspectionsRepository,
        IDiagnosesRepository diagnosesRepository,
        IConsultationsService consultationsService,
        IPatientsRepository patientsRepository)
    {
        _inspectionsRepository = inspectionsRepository;
        _diagnosesRepository = diagnosesRepository;
        _consultationsService = consultationsService;
        _patientsRepository = patientsRepository;
    }

    public async Task<Guid?> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId)
    {
        var patient = await _patientsRepository.GetById(patientId);

        if (patient == null)
        {
            return null;
        }
        
        Inspection inspection = new Inspection(request.date, request.anamnesis, request.complaints, request.conclusion, 
            request.nextVisitDate, request.deathDate, request.previousInspectionId, patientId, doctorId);
        
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
}