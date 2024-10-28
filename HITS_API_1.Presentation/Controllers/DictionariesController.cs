using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/dictionary")]
public class DictionariesController : ControllerBase
{
    private readonly ISpecialitiesService _specialitiesService;

    public DictionariesController(ISpecialitiesService specialitiesService)
    {
        _specialitiesService = specialitiesService;
    }

    [HttpGet("speciality")]
    [AllowAnonymous]
    public async Task<ActionResult<GetSpecialitiesResponse>> GetSpecialities([FromQuery] GetSpecialitiesRequest request)
    {
        var (specialities, pagination) = await _specialitiesService.GetSpecialities(request.name, request.page, 
            request.size);

        if (specialities == null)
        {
            return BadRequest("Недопустимое значение page");
        }
        
        GetSpecialitiesResponse response = new GetSpecialitiesResponse(specialities, pagination);
        
        return Ok(response);
    }
}