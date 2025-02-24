using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/consultation")]
public class ConsultationsController(
    IConsultationsService consultationsService,
    IDoctorsRepository doctorsRepository,
    ICommentsService commentsService,
    IInspectionsService inspectionsService)
    : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<InspectionPagedListResponse>> GetInspectionsForConsultation(
        [FromQuery] GetFilteredInspectionsRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var (inspections, pagination) =
            await inspectionsService.GetInspectionsForConsultation(Guid.Parse(doctorId), request);

        InspectionPagedListResponse response = new InspectionPagedListResponse(inspections, pagination);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<GetConsultationByIdResponse>> GetConsultationById([FromRoute] Guid id)
    {
        var (consultation, comments, speciality) = await consultationsService.GetConsultationById(id);

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
                comment.Content, comment.AuthorId, author == null ? "Deleted" : author.Name, comment.ParentId);

            commentsResponse.Add(commentResponse);
        }

        GetConsultationByIdResponse responseFull = new GetConsultationByIdResponse(consultation.Id,
            consultation.CreateTime, consultation.InspectionId, speciality, commentsResponse);
        return Ok(responseFull);
    }

    [HttpPost("{id}/comment")]
    [Authorize]
    public async Task<ActionResult> AddCommentToConsultation([FromRoute] Guid id, [FromBody] AddCommentRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return Unauthorized();
        }

        var commentId = await commentsService.CreateComment(id, request, Guid.Parse(doctorId));
        return Ok(commentId.ToString());
    }

    [HttpPut("comment/{id}")]
    [Authorize]
    public async Task<ActionResult> RedactComment([FromRoute] Guid id, [FromBody] RedactCommentRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return Unauthorized();
        }

        await commentsService.RedactComment(id, request, Guid.Parse(doctorId));
        return Ok();
    }
}