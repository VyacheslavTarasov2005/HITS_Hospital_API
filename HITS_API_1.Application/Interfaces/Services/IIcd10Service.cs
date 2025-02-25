using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IIcd10Service
{
    Task<(List<Icd10Entity>, Pagination)> GetIcd10(GetIcd10Request queryRequest);
    Task<List<Icd10Entity>> GetRootsIcd10();
    Task ValidateIcdRoots(List<Guid> icdRoots);
}