using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IInspectionsService
{
    Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId);
    Task<GetInspectionResponse> GetInspectionById(Guid inspectionId);
    Task UpdateInspection(Guid inspectionId, RedactInspectionRequest request, Guid doctorId);
    Task<List<GetInspectionByRootResponse>> GetInspectionsByRoot(Guid rootId);
    Task<List<GetPatientInspectionsNoChildrenResponse>?> GetPatientInspectionsNoChildren(Guid patientId,
        String? filter);

    Task<(List<GetInspectionByRootResponse>, Pagination)> GetInspectionsForConsultation(Guid doctorId,
        GetFilteredInspectionsRequest request);

    Task<(List<GetInspectionByRootResponse>, Pagination)> GetPatientInspections(Guid patientId,
        GetFilteredInspectionsRequest request);
}