using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface ISpecialitiesRepository
{
    Task<Speciality?> GetById(Guid specialityId);
    Task<List<Speciality>> GetAllByNamePart(String name);
}