using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class Icd10Service(IIcd10Repository icd10Repository, IPaginationService paginationService)
    : IIcd10Service
{
    public async Task<(List<Icd10Entity>, Pagination)> GetIcd10(String? request, int? page, int? size)
    {
        var icd10Entities = await icd10Repository.GetAllByName(request ?? "");

        if (icd10Entities.Count == 0)
        {
            icd10Entities = await icd10Repository.GetAllByCode(request ?? "");
        }

        return paginationService.PaginateList(icd10Entities, page, size);
    }

    public async Task<List<Icd10Entity>> GetRootsIcd10()
    {
        return await icd10Repository.GetRoots();
    }
}