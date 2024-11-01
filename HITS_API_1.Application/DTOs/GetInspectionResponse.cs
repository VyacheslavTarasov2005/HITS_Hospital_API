using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetInspectionResponse(
    Guid id,
    DateTime createTime,
    DateTime date,
    String? anamnesis,
    String? complaints,
    String? treatment,
    Conclusion conclusion,
    DateTime? nextVisitDate,
    DateTime? deathDate,
    Guid? baseInspectionId,
    Guid? previousInspectionId,
    GetPatientByIdResponse patient,
    GetDoctorResponse doctor,
    List<GetDiagnosisResponse>? diagnoses,
    List<InspectionConsultationModel>? consultations);