using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record CreateDiagnosisModel(
    Guid icdDiagnosisId,
    String? description,
    DiagnosisType type);