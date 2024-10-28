using HITS_API_1.Application.Entities;
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

    public async Task<(List<Icd10Entity>?, Pagination)> GetIcd10(String? request, int page, int size)
    {
        var icd10Entities = await _icd10Repository.GetAllByName(request ?? "");

        if (icd10Entities.Count == 0)
        {
            icd10Entities = await _icd10Repository.GetAllByCode(request ?? "");
        }
        
        Pagination pagination = new Pagination(size, icd10Entities.Count, page);

        if (icd10Entities.Count() == 0)
        {
            return (icd10Entities, pagination);
        }
        
        if (size * (page - 1) + 1 > icd10Entities.Count)
        {
            return (null, pagination);
        }
        
        List<Icd10Entity> icd10EntitiesPaginated = new List<Icd10Entity>();
        
        for (int i = size * (page - 1); i < int.Min(size * page, icd10Entities.Count); i++)
        {
            icd10EntitiesPaginated.Add(icd10Entities[i]);
        }
        
        return (icd10EntitiesPaginated, pagination);
    }

    public async Task<List<Icd10Entity>> GetRootsIcd10()
    {
        return await _icd10Repository.GetRoots();
    }
}