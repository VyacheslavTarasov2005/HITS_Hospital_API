namespace HITS_API_1.Application.DTOs;

public record InspectionCommentModel(
    Guid id,
    DateTime createTime,
    Guid? parentId,
    String? content,
    GetDoctorResponse author,
    DateTime modifyTime);