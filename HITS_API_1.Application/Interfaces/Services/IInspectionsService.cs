using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IInspectionsService
{
    Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId);
}