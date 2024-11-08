using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class SpecialitiesService : ISpecialitiesService
{
    private readonly ISpecialitiesRepository _specialitiesRepository;
    private readonly IPaginationService _paginationService;

    public SpecialitiesService(
        ISpecialitiesRepository specialitiesRepository,
        IPaginationService paginationService)
    {
        _specialitiesRepository = specialitiesRepository;
        _paginationService = paginationService;
    }

    public async Task<(List<Speciality>, Pagination)> GetSpecialities(String? name, int? page, int? size)
    {
        var specialities = await _specialitiesRepository.GetAllByName(name ?? "");

        return _paginationService.PaginateList(specialities, page, 
            size);
    }
}