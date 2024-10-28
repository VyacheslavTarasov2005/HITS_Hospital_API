using HITS_API_1.Domain;
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

    public async Task<List<Speciality>?> GetSpecialities(String? name, int page, int size)
    {
        var specialities = await _specialitiesRepository.GetAllByName(name ?? "");

        if (specialities.Count == 0)
        {
            return specialities;
        }

        if (size * (page - 1) + 1 > specialities.Count)
        {
            return null;
        }
        
        List<Speciality> specialitiesPaginated = new List<Speciality>();

        for (int i = size * (page - 1); i < int.Min(size * page, specialities.Count); i++)
        {
            specialitiesPaginated.Add(specialities[i]);
        }
        
        return specialitiesPaginated;
    }
}