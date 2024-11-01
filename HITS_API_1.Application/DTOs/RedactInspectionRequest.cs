using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record RedactInspectionRequest(
    String anamnesis,
    String complaints,
    String treatment,
    Conclusion conclusion,
    DateTime? nextVisitDate,
    DateTime? deathDate,
    List<CreateDiagnosisModel> diagnoses);