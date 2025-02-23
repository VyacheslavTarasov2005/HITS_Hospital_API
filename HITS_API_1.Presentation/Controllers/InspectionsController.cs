using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/inspection")]
public class InspectionsController(IInspectionsService inspectionsService) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetInspectionResponse>> GetInspectionById([FromRoute] Guid id)
    {
        var response = await inspectionsService.GetInspectionById(id);
        return Ok(response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> RedactInspection([FromRoute] Guid id, [FromBody] RedactInspectionRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return Unauthorized();
        }
        
        await inspectionsService.UpdateInspection(id, request, Guid.Parse(doctorId));
        return Ok();
    }

    [HttpGet("{id}/chain")]
    [Authorize]
    public async Task<ActionResult<GetInspectionByRootResponse>> GetInspectionByRoot([FromRoute] Guid id)
    {
        var inspections = await inspectionsService.GetInspectionsByRoot(id);
        return Ok(inspections);
    }
}