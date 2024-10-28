using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class Icd10Service : IIcd10Service
{
    private readonly IIcd10Repository _icd10Repository;

    public Icd10Service(IIcd10Repository icd10Repository)
    {
        _icd10Repository = icd10Repository;
    }

    public async Task<List<Icd10Entity>> GetRootsIcd10()
    {
        return await _icd10Repository.GetRoots();
    }
}