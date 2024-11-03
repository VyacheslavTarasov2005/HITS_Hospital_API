using System.Security.Claims;
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientsController : ControllerBase
{
    private readonly IPatientsService _patientsService;
    private readonly IValidator<CreatePatientRequest> _patientValidator;
    private readonly IInspectionsService _inspectionsService;
    private readonly CreateInspectionRequestValidator _createInspectionRequestValidator;
    private readonly GetPatientsListRequestValidator _getPatientsListRequestValidator;

    public PatientsController(
        IPatientsService patientsService, 
        IValidator<CreatePatientRequest> patientValidator,
        IInspectionsService inspectionsService,
        CreateInspectionRequestValidator createInspectionRequestValidator,
        GetPatientsListRequestValidator getPatientsListRequestValidator)
    {
        _patientsService = patientsService;
        _patientValidator = patientValidator;
        _inspectionsService = inspectionsService;
        _createInspectionRequestValidator = createInspectionRequestValidator;
        _getPatientsListRequestValidator = getPatientsListRequestValidator;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        var validationResult = await _patientValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var patientId = await _patientsService.CreatePatient(request.name, request.birthday, request.gender);
        
        return Ok(patientId.ToString());
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<GetPatientsListResponse>> GetPatients([FromQuery] GetPatientsListRequest request)
    {
        var validationResult = _getPatientsListRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }
        
        int page = request.page ?? 1;
        int size = request.size ?? 5;

        bool onlyMine = request.onlyMine ?? false;
        
        bool visits = request.scheduledVisits ?? false;
        
        var (patients, pagination) = await _patientsService.GetPatients(request.name, request.conclusions, 
            request.sorting, visits, onlyMine ? Guid.Parse(doctorId) : null, page, size);
        
        if (patients == null)
        {
            return BadRequest("Недопустимое значение page");
        }
        
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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult> GetPatientById([FromRoute] Guid id)
    {
        var patient = await _patientsService.GetPatientById(id);

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
        var validationResult = await _createInspectionRequestValidator.ValidateAsync(request);

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
            var inspectionId = await _inspectionsService.CreateInspection(request, id, Guid.Parse(doctorId));
            return Ok(inspectionId.ToString());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/inspections/search")]
    [Authorize]
    public async Task<ActionResult<List<GetPatientInspectionsNoChildrenResponse>>> GetInspectionsNoChildren(
        [FromRoute] Guid id, [FromQuery] String? request)
    {
        var inspections = await 
            _inspectionsService.GetPatientInspectionsNoChildren(id, request);

        if (inspections == null)
        {
            return NotFound("Пациент не найден");
        }
        
        return Ok(inspections);
    }
}