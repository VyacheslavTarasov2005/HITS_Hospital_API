using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/dictionary")]
public class DictionariesController(
    ISpecialitiesService specialitiesService,
    IIcd10Service icd10Service)
    : ControllerBase
{
    [HttpGet("speciality")]
    [AllowAnonymous]
    public async Task<ActionResult<GetSpecialitiesResponse>> GetSpecialities([FromQuery] GetSpecialitiesRequest request)
    {
        var (specialities, pagination) = await specialitiesService.GetSpecialities(request);

        var response = new GetSpecialitiesResponse(specialities, pagination);
        return Ok(response);
    }

    [HttpGet("icd10")]
    [AllowAnonymous]
    public async Task<ActionResult<GetIcd10Response>> GetIcd10([FromQuery] GetIcd10Request queryRequest)
    {
        var (icd10Entities, pagination) = await icd10Service.GetIcd10(queryRequest);

        var responseList = icd10Entities.Select(root => new Icd10Response(
            root.Code,
            root.Name,
            root.Id,
            root.CreateTime)
        ).ToList();

        var response = new GetIcd10Response(responseList, pagination);
        return Ok(response);
    }


    [HttpGet("icd10/roots")]
    [AllowAnonymous]
    public async Task<ActionResult<Icd10Response>> GetIcd10Roots()
    {
        var icd10RootsList = await icd10Service.GetRootsIcd10();

        var response = icd10RootsList.Select(root => new Icd10Response(
            root.Code,
            root.Name,
            root.Id,
            root.CreateTime)
        ).ToList();

        return Ok(response);
    }
}