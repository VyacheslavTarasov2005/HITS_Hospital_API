using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IPatientsService
{
    Task<Guid> CreatePatient(CreatePatientRequest request);
    Task<Patient> GetPatientById(Guid id);
    Task<(List<Patient>, Pagination)> GetPatients(GetPatientsListRequest request, Guid? doctorId);
    Task<GetReportResponse> GetPatientsReport(GetReportRequest request);
}