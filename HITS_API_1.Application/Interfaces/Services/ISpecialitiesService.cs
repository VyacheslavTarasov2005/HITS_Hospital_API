using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface ISpecialitiesService
{
    Task<(List<Speciality>, Pagination)> GetSpecialities(String? name, int? page, int? size);
}