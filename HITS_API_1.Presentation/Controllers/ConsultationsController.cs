using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/consultation")]
public class ConsultationsController(
    IConsultationsService consultationsService,
    ISpecialitiesRepository specialitiesRepository,
    IDoctorsRepository doctorsRepository,
    AddCommentRequestValidator addCommentRequestValidator,
    IConsultationsRepository consultationsRepository,
    ICommentsService commentsService,
    RedactCommentRequestValidator redactCommentRequestValidator,
    ICommentsRepository commentsRepository,
    GetFilteredInspectionsRequestValidator getFilteredInspectionsRequestValidator,
    IInspectionsService inspectionsService)
    : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<InspectionPagedListResponse>> GetInspectionsForConsultation(
        [FromQuery] GetFilteredInspectionsRequest request)
    {
        var validationResult = await getFilteredInspectionsRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }
        
        var doctor = await doctorsRepository.GetById(Guid.Parse(doctorId));

        if (doctor == null)
        {
            return Unauthorized();
        }

        try
        {
            var (inspections, pagination) = await inspectionsService.GetInspectionsForConsultation(
                doctor, request.grouped, request.icdRoots, request.page, request.size);
        
            InspectionPagedListResponse response = new InspectionPagedListResponse(inspections, pagination);
        
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetConsultationByIdResponse>> GetConsultationById([FromRoute] Guid id)
    {
        var (consultation, comments) = await consultationsService.GetConsultationById(id);

        if (consultation == null)
        {
            return NotFound("Консультация не найдена");
        }
        
        Speciality speciality = await specialitiesRepository.GetById(consultation.SpecialityId);
        
        if (comments.Count == 0)
        {
            GetConsultationByIdResponse response = new GetConsultationByIdResponse(consultation.Id, 
                consultation.CreateTime, consultation.InspectionId, speciality, null);
            
            return Ok(response);
        }
        
        List<GetCommentModel> commentsResponse = new List<GetCommentModel>();

        foreach (var comment in comments)
        {
            var author = await doctorsRepository.GetById(comment.AuthorId);
            
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
        var validationResult = addCommentRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }
        
        var consultation = await consultationsRepository.GetById(id);

        if (consultation == null)
        {
            return NotFound("Консультация не найдена");
        }
        
        var doctor = await doctorsRepository.GetById(Guid.Parse(doctorId));

        if (doctor == null)
        {
            return Unauthorized();
        }

        if (consultation.SpecialityId != doctor.Speciality)
        {
            return StatusCode(403, "Пользователь не может добавлять комментарии к этой консультации");
        }
        
        try
        {
            var commentId = await commentsService.CreateComment(request.content, request.parentId, id, 
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

    [HttpPut("comment/{id}")]
    [Authorize]
    public async Task<ActionResult> RedactComment([FromRoute] Guid id, [FromBody] RedactCommentRequest request)
    {
        var validationResult = redactCommentRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var comment = await commentsRepository.GetById(id);

        if (comment == null)
        {
            return NotFound("Комментарий не найден");
        }

        if (comment.Content == request.content)
        {
            return BadRequest("Нельзя изменять содержимое комментария на такое же");
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Unauthorized();
        }

        if (comment.AuthorId != Guid.Parse(doctorId))
        {
            return StatusCode(403, "Пользователь может изменять только свои комментарии"); 
        }
        
        await commentsService.RedactComment(id, request.content);

        return Ok();
    }
}