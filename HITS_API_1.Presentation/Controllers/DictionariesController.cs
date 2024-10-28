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
    private readonly IIcd10Service _icd10Service;

    public DictionariesController(ISpecialitiesService specialitiesService, IIcd10Service icd10Service)
    {
        _specialitiesService = specialitiesService;
        _icd10Service = icd10Service;
    }

    [HttpGet("speciality")]
    [AllowAnonymous]
    public async Task<ActionResult<GetSpecialitiesResponse>> GetSpecialities([FromQuery] GetSpecialitiesRequest request)
    {
        var (specialities, pagination) = await _specialitiesService.GetSpecialities(request.name, 
            request.page, request.size);

        if (specialities == null)
        {
            return BadRequest("Недопустимое значение page");
        }
        
        GetSpecialitiesResponse response = new GetSpecialitiesResponse(specialities, pagination);
        
        return Ok(response);
    }

    [HttpGet("icd10")]
    [AllowAnonymous]
    public async Task<ActionResult<GetIcd10Response>> GetIcd10([FromQuery] GetIcd10Request queryRequest)
    {
        var (icd10Entities, pagination) = await _icd10Service.GetIcd10(queryRequest.request, 
            queryRequest.page, queryRequest.size);

        if (icd10Entities == null)
        {
            return BadRequest("Недопустимое значение page");
        }
        
        var responseList = icd10Entities.Select(root => new Icd10Response(
            root.Code,
            root.Name,
            root.Id,
            root.CreateTime)
        ).ToList();
        
        GetIcd10Response response = new GetIcd10Response(responseList, pagination);
        return Ok(response);
    }
    

    [HttpGet("icd10/roots")]
    [AllowAnonymous]
    public async Task<ActionResult<Icd10Response>> GetIcd10Roots()
    {
        var icd10RootsList = await _icd10Service.GetRootsIcd10();
        
        var response = icd10RootsList.Select(root => new Icd10Response(
            root.Code,
            root.Name,
            root.Id,
            root.CreateTime)
        ).ToList();
        
        return Ok(response);
    }
}