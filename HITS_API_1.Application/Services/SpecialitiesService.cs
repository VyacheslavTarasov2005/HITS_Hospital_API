using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class SpecialitiesService : ISpecialitiesService
{
    private readonly ISpecialitiesRepository _specialitiesRepository;

    public SpecialitiesService(ISpecialitiesRepository specialitiesRepository)
    {
        _specialitiesRepository = specialitiesRepository;
    }

    public async Task<(List<Speciality>?, Pagination)> GetSpecialities(String? name, int page, int size)
    {
        var specialities = await _specialitiesRepository.GetAllByName(name ?? "");
        
        Pagination pagination = new Pagination(size, specialities.Count, page);

        if (specialities.Count == 0)
        {
            return (specialities, pagination);
        }

        if (size * (page - 1) + 1 > specialities.Count)
        {
            return (null, pagination);
        }
        
        List<Speciality> specialitiesPaginated = new List<Speciality>();

        for (int i = size * (page - 1); i < int.Min(size * page, specialities.Count); i++)
        {
            specialitiesPaginated.Add(specialities[i]);
        }
        
        return (specialitiesPaginated, pagination);
    }
}