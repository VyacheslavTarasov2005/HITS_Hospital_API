using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/consultation")]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationsService _consultationsService;
    private readonly ISpecialitiesRepository _specialitiesRepository;
    private readonly IDoctorsRepository _doctorsRepository;

    public ConsultationsController(
        IConsultationsService consultationsService, 
        ISpecialitiesRepository specialitiesRepository,
        IDoctorsRepository doctorsRepository)
    {
        _consultationsService = consultationsService;
        _specialitiesRepository = specialitiesRepository;
        _doctorsRepository = doctorsRepository;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetConsultationByIdResponse>> GetConsultationById([FromRoute] Guid id)
    {
        var (consultation, comments) = await _consultationsService.GetConsultationById(id);

        if (consultation == null)
        {
            return NotFound("Консультация не найдена");
        }
        
        Speciality speciality = await _specialitiesRepository.GetById(consultation.SpecialityId);
        
        if (comments.Count == 0)
        {
            GetConsultationByIdResponse response = new GetConsultationByIdResponse(consultation.Id, 
                consultation.CreateTime, consultation.InspectionId, speciality, null);
            
            return Ok(response);
        }
        
        List<GetCommentModel> commentsResponse = new List<GetCommentModel>();

        foreach (var comment in comments)
        {
            var author = await _doctorsRepository.GetById(comment.AuthorId);
            
            GetCommentModel commentResponse = new GetCommentModel(comment.Id, comment.CreateTime, comment.ModifiedDate,
                comment.Content, comment.AuthorId, author.Name, comment.ParentId);
            
            commentsResponse.Add(commentResponse);
        }
        
        GetConsultationByIdResponse responseFull = new GetConsultationByIdResponse(consultation.Id, 
            consultation.CreateTime, consultation.InspectionId, speciality, commentsResponse);
        
        return Ok(responseFull);
    }
}