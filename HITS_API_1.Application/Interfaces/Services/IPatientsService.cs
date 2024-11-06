using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IPatientsService
{
    Task<Guid> CreatePatient(String name, DateTime? birthday, Gender gender);
    Task<Patient?> GetPatientById(Guid id);
    Task<(List<Patient>?, Pagination)> GetPatients(String? name, List<Conclusion>? conclusions, Sorting? sorting,
        bool scheduledVisits, Guid? doctorId, int page, int size);
    Task<GetReportResponse> GetPatientsReport(GetReportRequest request);
}