using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/report")]
public class ReportsController(IPatientsService patientsService) : ControllerBase
{
    [HttpGet("icdrootsreport")]
    [Authorize]
    public async Task<ActionResult<GetReportResponse>> GetReport([FromQuery] GetReportRequest request)
    {
        var response = await patientsService.GetPatientsReport(request);
        return Ok(response);
    }
}