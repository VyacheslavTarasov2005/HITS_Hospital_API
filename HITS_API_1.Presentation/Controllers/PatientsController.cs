using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientsController : ControllerBase
{
    private readonly IPatientsService _patientsService;
    private readonly IValidator<CreatePatientRequest> _patientValidator;

    public PatientsController(IPatientsService patientsService, IValidator<CreatePatientRequest> patientValidator)
    {
        _patientsService = patientsService;
        _patientValidator = patientValidator;
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
}