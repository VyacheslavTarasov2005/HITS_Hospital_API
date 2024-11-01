using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record InspectionConsultationModel(
    Guid id,
    DateTime createTime,
    Guid inspectionId,
    Speciality speciality,
    InspectionCommentModel rootComment,
    int commentsNumber);