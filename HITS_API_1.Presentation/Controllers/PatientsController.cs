using System.Security.Claims;
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientsController(
    IPatientsService patientsService,
    IValidator<CreatePatientRequest> patientValidator,
    IInspectionsService inspectionsService,
    CreateInspectionRequestValidator createInspectionRequestValidator,
    GetPatientsListRequestValidator getPatientsListRequestValidator,
    GetFilteredInspectionsRequestValidator getFilteredInspectionsRequestValidator,
    IPatientsRepository patientsRepository)
    : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        var validationResult = await patientValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var patientId = await patientsService.CreatePatient(request.name, request.birthday, request.gender);
        
        return Ok(patientId.ToString());
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<GetPatientsListResponse>> GetPatients([FromQuery] GetPatientsListRequest request)
    {
        var validationResult = getPatientsListRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }

        bool onlyMine = request.onlyMine ?? false;
        
        bool visits = request.scheduledVisits ?? false;

        try
        {
            var (patients, pagination) = await patientsService.GetPatients(request.name, request.conclusions, 
                request.sorting, visits, onlyMine ? Guid.Parse(doctorId) : null, request.page, request.size);
            
            List<GetPatientByIdResponse> patientsResponse = new List<GetPatientByIdResponse>();

            foreach (var patient in patients)
            {
                GetPatientByIdResponse patientResponse = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
                    patient.Name, patient.Birthday, patient.Sex);
            
                patientsResponse.Add(patientResponse);
            }
        
            GetPatientsListResponse response = new GetPatientsListResponse(patientsResponse, pagination);
        
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetPatientById([FromRoute] Guid id)
    {
        var patient = await patientsService.GetPatientById(id);

        if (patient == null)
        {
            return NotFound();
        }
        
        GetPatientByIdResponse response = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
            patient.Name, patient.Birthday, patient.Sex);
        
        return Ok(response);
    }

    [HttpPost("{id}/inspections")]
    [Authorize]
    public async Task<ActionResult> CreateInspection([FromRoute] Guid id,
        [FromBody] CreateInspectionRequest request)
    {
        var validationResult = await createInspectionRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }

        try
        {
            var inspectionId = await inspectionsService.CreateInspection(request, id, Guid.Parse(doctorId));
            return Ok(inspectionId.ToString());
        }
        catch (Exception e)
        {
            if (e is NullReferenceException)
            {
                return NotFound(e.Message);
            }
            
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/inspections/search")]
    [Authorize]
    public async Task<ActionResult<List<GetPatientInspectionsNoChildrenResponse>>> GetInspectionsNoChildren(
        [FromRoute] Guid id, [FromQuery] String? request)
    {
        var inspections = await 
            inspectionsService.GetPatientInspectionsNoChildren(id, request);

        if (inspections == null)
        {
            return NotFound("Пациент не найден");
        }
        
        return Ok(inspections);
    }

    [HttpGet("{id}/inspections")]
    [Authorize]
    public async Task<ActionResult<InspectionPagedListResponse>> GetInspections([FromRoute] Guid id,
        [FromQuery] GetFilteredInspectionsRequest request)
    {
        var validationResult = await getFilteredInspectionsRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var patient = await patientsRepository.GetById(id);

        if (patient == null)
        {
            return NotFound("Пациент не найден");
        }

        try
        {
            var (inspections, pagination) = await 
                inspectionsService.GetPatientInspections(patient, request.grouped, request.icdRoots, request.page, 
                    request.size);
        
            InspectionPagedListResponse response = new InspectionPagedListResponse(inspections, pagination);
        
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}