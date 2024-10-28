using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IIcd10Service
{
    Task<(List<Icd10Entity>?, Pagination)> GetIcd10(String? request, int page, int size);
    Task<List<Icd10Entity>> GetRootsIcd10();
}