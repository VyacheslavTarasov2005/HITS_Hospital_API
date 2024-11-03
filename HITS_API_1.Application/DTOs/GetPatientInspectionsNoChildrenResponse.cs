namespace HITS_API_1.Application.DTOs;

public record GetPatientInspectionsNoChildrenResponse(
    Guid id,
    DateTime createTime,
    DateTime date,
    GetDiagnosisResponse diagnosis);