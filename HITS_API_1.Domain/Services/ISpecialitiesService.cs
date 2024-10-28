using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain;

public interface ISpecialitiesService
{
    Task<List<Speciality>?> GetSpecialities(String? name, int page, int size);
}