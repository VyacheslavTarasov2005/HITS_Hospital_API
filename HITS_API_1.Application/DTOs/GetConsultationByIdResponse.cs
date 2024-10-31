using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetConsultationByIdResponse(
    Guid id,
    DateTime createTime,
    Guid inspectionId,
    Speciality speciality,
    List<GetCommentModel>? comments);