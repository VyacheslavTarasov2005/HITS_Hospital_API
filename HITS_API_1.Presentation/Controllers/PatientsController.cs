using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/patient")]
public class PatientsController(
    IPatientsService patientsService,
    IInspectionsService inspectionsService)
    : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreatePatient([FromBody] CreatePatientRequest request)
    {
        var patientId = await patientsService.CreatePatient(request);
        return Ok(patientId.ToString());
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<GetPatientsListResponse>> GetPatients([FromQuery] GetPatientsListRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return Unauthorized();
        }

        bool onlyMine = request.onlyMine ?? false;

        var (patients, pagination) = await patientsService.GetPatients(request,
            onlyMine ? Guid.Parse(doctorId) : null);

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
        var patient = await patientsService.GetPatientById(id);

        GetPatientByIdResponse response = new GetPatientByIdResponse(patient.Id, patient.CreateTime,
            patient.Name, patient.Birthday, patient.Sex);
        return Ok(response);
    }

    [HttpPost("{id}/inspections")]
    [Authorize]
    public async Task<ActionResult> CreateInspection([FromRoute] Guid id,
        [FromBody] CreateInspectionRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return Unauthorized();
        }

        var inspectionId = await inspectionsService.CreateInspection(request, id, Guid.Parse(doctorId));
        return Ok(inspectionId.ToString());
    }

    [HttpGet("{id}/inspections/search")]
    [Authorize]
    public async Task<ActionResult<List<GetPatientInspectionsNoChildrenResponse>>> GetInspectionsNoChildren(
        [FromRoute] Guid id, [FromQuery] String? request)
    {
        var inspections = await
            inspectionsService.GetPatientInspectionsNoChildren(id, request);

        return Ok(inspections);
    }

    [HttpGet("{id}/inspections")]
    [Authorize]
    public async Task<ActionResult<InspectionPagedListResponse>> GetInspections([FromRoute] Guid id,
        [FromQuery] GetFilteredInspectionsRequest request)
    {
        var (inspections, pagination) = await
            inspectionsService.GetPatientInspections(id, request);

        InspectionPagedListResponse response = new InspectionPagedListResponse(inspections, pagination);
        return Ok(response);
    }
}