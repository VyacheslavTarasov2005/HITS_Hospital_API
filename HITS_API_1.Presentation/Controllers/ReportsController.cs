using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/report")]
public class ReportsController : ControllerBase
{
    private readonly GetReportRequestValidator _getReportRequestValidator;
    private readonly IPatientsService _patientsService;

    public ReportsController(GetReportRequestValidator getReportRequestValidator, IPatientsService patientsService)
    {
        _getReportRequestValidator = getReportRequestValidator;
        _patientsService = patientsService;
    }

    [HttpGet("icdrootsreport")]
    [Authorize]
    public async Task<ActionResult<GetReportResponse>> GetReport([FromQuery] GetReportRequest request)
    {
        var validationResult = await _getReportRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var response = await _patientsService.GetPatientsReport(request);
        
        return Ok(response);
    }
}