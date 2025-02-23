using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IDiagnosesService
{
    Task<List<GetDiagnosisResponse>> GetDiagnosesByInspection(Guid inspectionId);
    Task ValidateDiagnoses(List<CreateDiagnosisModel> diagnoses);
}