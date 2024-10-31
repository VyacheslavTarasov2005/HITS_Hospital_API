namespace HITS_API_1.Application.DTOs;

public record CreateConsultationModel(
    Guid specialityId,
    CreateInspectionCommentModel comment);
