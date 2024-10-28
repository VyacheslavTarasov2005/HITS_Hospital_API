using FluentValidation;
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
    private readonly IValidator<GetSpecialitiesRequest> _getSpecialitiesRequestValidator;
    private readonly IValidator<GetIcd10Request> _getIcd10RequestValidator;

    public DictionariesController(
        ISpecialitiesService specialitiesService, 
        IIcd10Service icd10Service,
        IValidator<GetSpecialitiesRequest> getSpecialitiesRequestValidator, 
        IValidator<GetIcd10Request> getIcd10RequestValidator)
    {
        _specialitiesService = specialitiesService;
        _icd10Service = icd10Service;
        _getSpecialitiesRequestValidator = getSpecialitiesRequestValidator;
        _getIcd10RequestValidator = getIcd10RequestValidator;
    }

    [HttpGet("speciality")]
    [AllowAnonymous]
    public async Task<ActionResult<GetSpecialitiesResponse>> GetSpecialities([FromQuery] GetSpecialitiesRequest request)
    {
        var validationResult = await _getSpecialitiesRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
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
        var validationResult = await _getIcd10RequestValidator.ValidateAsync(queryRequest);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
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