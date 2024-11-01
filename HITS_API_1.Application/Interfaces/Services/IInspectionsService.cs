using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IInspectionsService
{
    Task<Guid> CreateInspection(CreateInspectionRequest request, Guid patientId, Guid doctorId);
    Task<(Inspection?, Inspection?)> GetInspectionByIdWithBaseInspection(Guid inspectionId);
}