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
public class InspectionsController(
    IInspectionsService inspectionsService,
    IPatientsService patientsService,
    IDoctorsService doctorsService,
    IDiagnosesService diagnosesService,
    IConsultationsService consultationsService,
    RedactInspectionRequestValidator redactInspectionRequestValidator)
    : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetInspectionResponse>> GetInspectionById([FromRoute] Guid id)
    {
        var inspection = await inspectionsService.GetInspectionById(id);

        if (inspection == null)
        {
            return NotFound("Осмотр не найден");
        }
        
        var baseInspection = await inspectionsService.GetBaseInspection(inspection);
        
        var patient = await patientsService.GetPatientById(inspection.PatientId);

        GetPatientByIdResponse patientResponse = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
            patient.Name, patient.Birthday, patient.Sex);
        
        var doctor = await doctorsService.GetDoctor(inspection.DoctorId);
        
        GetDoctorResponse doctorResponse = new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name, 
            doctor.Birthday, doctor.Sex, doctor.Email, doctor.Phone);
        
        var diagnoses = await diagnosesService.GetDiagnosesByInspection(inspection.Id);

        if (diagnoses.Count == 0)
        {
            diagnoses = null;
        }
        
        var consultations = await 
            consultationsService.GetAllConsultationsByInspection(inspection.Id);

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
        var validationResult = await redactInspectionRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var inspection = await inspectionsService.GetInspectionById(id);

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
            return StatusCode(403, "Пользователь не может редактировать этот осмотр");
        }

        try
        {
            await inspectionsService.UpdateInspection(request, inspection);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    [HttpGet("{id}/chain")]
    [Authorize]
    public async Task<ActionResult<GetInspectionByRootResponse>> GetInspectionByRoot([FromRoute] Guid id)
    {
        try
        {
            var inspections = await inspectionsService.GetInspectionsByRoot(id);
            return Ok(inspections);
        }
        catch (Exception ex)
        {
            if (ex is NullReferenceException)
            {
                return NotFound(ex.Message);
            }
            
            return BadRequest(ex.Message);
        }
    }
}