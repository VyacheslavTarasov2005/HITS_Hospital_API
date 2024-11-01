using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DiagnosesService : IDiagnosesService
{
    private readonly IDiagnosesRepository _diagnosesRepository;
    private readonly IIcd10Repository _icd10Repository;

    public DiagnosesService(IDiagnosesRepository diagnosesRepository, IIcd10Repository icd10Repository)
    {
        _diagnosesRepository = diagnosesRepository;
        _icd10Repository = icd10Repository;
    }

    public async Task<List<GetDiagnosisResponse>> GetDiagnosesByInspection(Guid inspectionId)
    {
        var diagnoses = await _diagnosesRepository.GetAllByInspection(inspectionId);

        List<GetDiagnosisResponse> response = new List<GetDiagnosisResponse>();

        foreach (var diagnosis in diagnoses)
        {
            var icd = await _icd10Repository.GetById(diagnosis.Icd10Id);
            
            GetDiagnosisResponse diagnosisResponse = new GetDiagnosisResponse(diagnosis.Id, diagnosis.CreateTime,
                icd.Code, icd.Name, diagnosis.Description, diagnosis.Type);
            
            response.Add(diagnosisResponse);
        }
        
        return response;
    }
}