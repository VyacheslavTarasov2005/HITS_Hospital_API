using HITS_API_1.Application.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetInspectionsForConsultationResponse(
    List<GetInspectionByRootResponse> inspections,
    Pagination pagination);
