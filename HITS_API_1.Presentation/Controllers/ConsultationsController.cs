using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/consultation")]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationsService _consultationsService;
    private readonly ISpecialitiesRepository _specialitiesRepository;
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly AddCommentRequestValidator _addCommentRequestValidator;
    private readonly IConsultationsRepository _consultationsRepository;
    private readonly ICommentsService _commentsService;

    public ConsultationsController(
        IConsultationsService consultationsService, 
        ISpecialitiesRepository specialitiesRepository,
        IDoctorsRepository doctorsRepository,
        AddCommentRequestValidator addCommentRequestValidator,
        IConsultationsRepository consultationsRepository,
        ICommentsService commentsService)
    {
        _consultationsService = consultationsService;
        _specialitiesRepository = specialitiesRepository;
        _doctorsRepository = doctorsRepository;
        _addCommentRequestValidator = addCommentRequestValidator;
        _consultationsRepository = consultationsRepository;
        _commentsService = commentsService;
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

    [HttpPost("{id}/comment")]
    [Authorize]
    public async Task<ActionResult> AddCommentToConsulatation([FromRoute] Guid id, [FromBody] AddCommentRequest request)
    {
        var validationResult = _addCommentRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }
        
        var consultation = await _consultationsRepository.GetById(id);

        if (consultation == null)
        {
            return NotFound("Консультация не найдена");
        }
        
        var doctor = await _doctorsRepository.GetById(Guid.Parse(doctorId));

        if (doctor == null)
        {
            return Unauthorized();
        }

        if (consultation.SpecialityId != doctor.Speciality)
        {
            return StatusCode(403, "Доктор не может добавлять комментарии к этой консультации");
        }
        
        try
        {
            var commentId = await _commentsService.CreateComment(request.content, request.parentId, id, 
                Guid.Parse(doctorId));

            return Ok(commentId.ToString());
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(NullReferenceException))
            {
                return NotFound(e.Message);
            }
            
            return BadRequest(e.Message);
        }
    }
}