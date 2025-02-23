using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Exceptions;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class Icd10Service(
    IIcd10Repository icd10Repository, 
    IPaginationService paginationService,
    GetIcd10RequestValidator getIcd10RequestValidator)
    : IIcd10Service
{
    public async Task<(List<Icd10Entity>, Pagination)> GetIcd10(GetIcd10Request getIcd10Request)
    {
        var validationResult = await getIcd10RequestValidator.ValidateAsync(getIcd10Request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var icd10Entities = await icd10Repository.GetAllByNamePart(getIcd10Request.request ?? "");

        if (icd10Entities.Count == 0)
        {
            icd10Entities = await icd10Repository.GetAllByCodePart(getIcd10Request.request ?? "");
        }

        return paginationService.PaginateList(icd10Entities, getIcd10Request.page, getIcd10Request.size);
    }

    public async Task<List<Icd10Entity>> GetRootsIcd10()
    {
        return await icd10Repository.GetRoots();
    }
    
    public async Task ValidateIcdRoots(List<Guid> icdRoots)
    {
        var roots = await icd10Repository.GetRoots();

        foreach (var icdRoot in icdRoots)
        {
            if (roots.Find(r => r.Id == icdRoot) == null)
            {
                throw new IncorrectFieldException("icdRoots", $"ICD {icdRoot} не является корневым эллементом МКБ");
            }
        }
    }
}