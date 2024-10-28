using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IIcd10Repository
{
    Task Load();
    Task<List<Icd10Entity>> GetRoots();
}