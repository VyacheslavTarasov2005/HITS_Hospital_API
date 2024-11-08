using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DiagnosesService(IDiagnosesRepository diagnosesRepository, IIcd10Repository icd10Repository)
    : IDiagnosesService
{
    public async Task<List<GetDiagnosisResponse>> GetDiagnosesByInspection(Guid inspectionId)
    {
        var diagnoses = await diagnosesRepository.GetAllByInspection(inspectionId);

        List<GetDiagnosisResponse> response = new List<GetDiagnosisResponse>();

        foreach (var diagnosis in diagnoses)
        {
            var icd = await icd10Repository.GetById(diagnosis.Icd10Id);
            
            GetDiagnosisResponse diagnosisResponse = new GetDiagnosisResponse(diagnosis.Id, diagnosis.CreateTime,
                icd.Code, icd.Name, diagnosis.Description, diagnosis.Type);
            
            response.Add(diagnosisResponse);
        }
        
        return response;
    }
}