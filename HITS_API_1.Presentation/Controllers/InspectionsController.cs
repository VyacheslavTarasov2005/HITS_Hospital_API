using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/inspection")]
public class InspectionsController : ControllerBase
{
    private readonly IInspectionsService _inspectionsService;
    private readonly IPatientsService _patientsService;
    private readonly IDoctorsService _doctorsService;
    private readonly IDiagnosesService _diagnosesService;
    private readonly IConsultationsService _consultationsService;

    public InspectionsController(
        IInspectionsService inspectionsService, 
        IPatientsService patientsService, 
        IDoctorsService doctorsService,
        IDiagnosesService diagnosesService,
        IConsultationsService consultationsService)
    {
        _inspectionsService = inspectionsService;
        _patientsService = patientsService;
        _doctorsService = doctorsService;
        _diagnosesService = diagnosesService;
        _consultationsService = consultationsService;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetInspectionResponse>> GetInspectionById([FromRoute] Guid id)
    {
        var (inspection, baseInspection) = await _inspectionsService.GetInspectionByIdWithBaseInspection(id);

        if (inspection == null)
        {
            return NotFound("Осмотр не найден");
        }
        
        var patient = await _patientsService.GetPatientById(inspection.PatientId);

        GetPatientByIdResponse patientResponse = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
            patient.Name, patient.Birthday, patient.Sex);
        
        var doctor = await _doctorsService.GetDoctor(inspection.DoctorId);
        
        GetDoctorResponse doctorResponse = new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name, 
            doctor.Birthday, doctor.Sex, doctor.Email, doctor.Phone);
        
        var diagnoses = await _diagnosesService.GetDiagnosesByInspection(inspection.Id);

        if (diagnoses.Count == 0)
        {
            diagnoses = null;
        }
        
        var consultations = await 
            _consultationsService.GetAllConsultationsByInspection(inspection.Id);

        if (consultations.Count == 0)
        {
            consultations = null;
        }
        
        GetInspectionResponse response = new GetInspectionResponse(inspection.Id, inspection.CreateTime,
            inspection.Date, inspection.Anamnesis, inspection.Complaints, inspection.Treatment, inspection.Conclusion,
            inspection.NextVisitDate, inspection.DeathDate, baseInspection?.Id, inspection.PreviousInspectionId,
            patientResponse, doctorResponse, diagnoses, consultations);

        return response;
    }
}