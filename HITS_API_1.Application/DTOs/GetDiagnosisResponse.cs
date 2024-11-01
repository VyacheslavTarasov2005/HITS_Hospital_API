using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetDiagnosisResponse(
    Guid id,
    DateTime createTime,
    String code,
    String name,
    String? description,
    DiagnosisType diagnosisType);