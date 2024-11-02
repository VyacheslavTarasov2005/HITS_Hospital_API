using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetInspectionByRootResponse(
    Guid id,
    DateTime createTime,
    Guid? previousId,
    DateTime date,
    Conclusion conclusion,
    Guid doctorId,
    String doctor,
    Guid patientId,
    String patient,
    GetDiagnosisResponse diagnosis,
    bool hasChain,
    bool hasNested);