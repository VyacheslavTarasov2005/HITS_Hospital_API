using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
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
    private readonly RedactInspectionRequestValidator _redactInspectionRequestValidator;

    public InspectionsController(
        IInspectionsService inspectionsService, 
        IPatientsService patientsService, 
        IDoctorsService doctorsService,
        IDiagnosesService diagnosesService,
        IConsultationsService consultationsService,
        RedactInspectionRequestValidator redactInspectionRequestValidator)
    {
        _inspectionsService = inspectionsService;
        _patientsService = patientsService;
        _doctorsService = doctorsService;
        _diagnosesService = diagnosesService;
        _consultationsService = consultationsService;
        _redactInspectionRequestValidator = redactInspectionRequestValidator;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetInspectionResponse>> GetInspectionById([FromRoute] Guid id)
    {
        var inspection = await _inspectionsService.GetInspectionById(id);

        if (inspection == null)
        {
            return NotFound("Осмотр не найден");
        }
        
        var baseInspection = await _inspectionsService.GetBaseInspection(inspection);
        
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

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> RedactInspection([FromRoute] Guid id, [FromBody] RedactInspectionRequest request)
    {
        var validationResult = await _redactInspectionRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var inspection = await _inspectionsService.GetInspectionById(id);

        if (inspection == null)
        {
            return NotFound("Осмотр не найден");
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }

        if (inspection.DoctorId != Guid.Parse(doctorId))
        {
            return StatusCode(403, "Пользователь не редактировать этот осмотр");
        }
        
        if (request.nextVisitDate != null)
        {
            if (inspection.Date > request.nextVisitDate)
            {
                return BadRequest("Дата следующего визита не может быть раньше даты осмотра");
            }
        }

        if (request.deathDate != null)
        {
            if (inspection.PreviousInspectionId != null)
            {
                var previousInspection = await _inspectionsService.GetInspectionById(
                    inspection.PreviousInspectionId.Value);

                if (request.deathDate < previousInspection.Date)
                {
                    return BadRequest("Дата смерти не может быть раньше даты предыдущего осмотра");
                }
            }

            if (request.deathDate > inspection.Date)
            {
                return BadRequest("Дата смерти не может быть позже даты осмотра");
            }
        }

        await _inspectionsService.UpdateInspection(request, id);
        return Ok();
    }
}