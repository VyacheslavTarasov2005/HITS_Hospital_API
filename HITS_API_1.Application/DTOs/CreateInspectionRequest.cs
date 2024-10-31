using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record CreateInspectionRequest(
    DateTime date,
    String anamnesis,
    String complaints,
    String treatment,
    Conclusion conclusion,
    DateTime? nextVisitDate,
    DateTime? deathDate,
    Guid? previousInspectionId,
    List<CreateDiagnosisModel> diagnoses,
    List<CreateConsultationModel>? consultations);