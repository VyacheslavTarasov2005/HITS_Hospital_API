using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IInspectionsService
{
    Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId);
    Task<Inspection?> GetInspectionById(Guid inspectionId);
    Task<Inspection?> GetBaseInspection(Inspection inspection);
    Task UpdateInspection(RedactInspectionRequest request, Inspection inspection);
    Task<List<GetInspectionByRootResponse>> GetInspectionsByRoot(Guid rootId);
    Task<List<GetPatientInspectionsNoChildrenResponse>?> GetPatientInspectionsNoChildren(Guid patientId, 
        String? filter);

    Task<(List<GetInspectionByRootResponse>?, Pagination)> GetInspectionsForConsultation(Doctor doctor,
        bool? grouped, List<Guid>? icdRoots, int page, int size);

    Task<(List<GetInspectionByRootResponse>?, Pagination)> GetPatientInspections(Patient patient,
        bool? grouped, List<Guid>? icdRoots, int page, int size);
}