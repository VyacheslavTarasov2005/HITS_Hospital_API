using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class Icd10Service : IIcd10Service
{
    private readonly IIcd10Repository _icd10Repository;
    private readonly IPaginationService _paginationService;

    public Icd10Service(IIcd10Repository icd10Repository, IPaginationService paginationService)
    {
        _icd10Repository = icd10Repository;
        _paginationService = paginationService;
    }

    public async Task<(List<Icd10Entity>, Pagination)> GetIcd10(String? request, int? page, int? size)
    {
        var icd10Entities = await _icd10Repository.GetAllByName(request ?? "");

        if (icd10Entities.Count == 0)
        {
            icd10Entities = await _icd10Repository.GetAllByCode(request ?? "");
        }

        return _paginationService.PaginateList(icd10Entities, page, size);
    }

    public async Task<List<Icd10Entity>> GetRootsIcd10()
    {
        return await _icd10Repository.GetRoots();
    }
}