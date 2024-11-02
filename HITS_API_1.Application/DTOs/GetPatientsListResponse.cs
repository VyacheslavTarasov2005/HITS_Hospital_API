using HITS_API_1.Application.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetPatientsListResponse(
    List<GetPatientByIdResponse> patients,
    Pagination pagination);