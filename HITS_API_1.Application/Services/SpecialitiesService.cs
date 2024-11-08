using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class SpecialitiesService(
    ISpecialitiesRepository specialitiesRepository,
    IPaginationService paginationService)
    : ISpecialitiesService
{
    public async Task<(List<Speciality>, Pagination)> GetSpecialities(String? name, int? page, int? size)
    {
        var specialities = await specialitiesRepository.GetAllByName(name ?? "");

        return paginationService.PaginateList(specialities, page, 
            size);
    }
}