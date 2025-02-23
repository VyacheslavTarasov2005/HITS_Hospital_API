using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IIcd10Repository
{
    Task<List<Icd10Entity>> GetAllByNamePart(String name);
    Task<List<Icd10Entity>> GetAllByCodePart(String code);
    Task<List<Icd10Entity>> GetRoots();
    Task<Icd10Entity?> GetById(Guid id);
    Task<Icd10Entity?> GetRootByChildId(Guid childId);
}